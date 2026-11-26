#!/usr/bin/env python3
"""Refine the existing SWARM ships into 0.9.11.

The silhouette is intentionally preserved (the audit asked for that), while the
palette, accents, rim light, wing edges, cockpit/cannon read and margins are
reworked. Source pixels are the 0.9.10 PNGs; no external images.
"""
from PIL import Image
import os, json, sys

ROOT = os.path.dirname(os.path.abspath(__file__))
ORIG = os.path.join(ROOT, 'base')
OUT  = os.path.join(ROOT, 'export')
PREV = os.path.join(ROOT, 'preview')
W=H=32

OUTLINE=(2,4,10,255)
BODY=(77,255,224,255)
LIGHT=(166,255,242,255)
SHADOW=(26,143,134,255)
WHITE=(235,247,255,255)
ORANGE=(255,140,46,255)
GREEN=(51,255,179,255)
YELLOW=(255,224,102,255)
CLEAR=(0,0,0,0)

SHIP_ACCENT={'baluarte':ORANGE,'vetor':GREEN,'lanca':YELLOW}

def frames(path, fw, fh):
    im=Image.open(path).convert('RGBA')
    return [im.crop((i*fw,0,(i+1)*fw,fh)) for i in range(im.width//fw)]

def near(c, target, tol=8):
    return all(abs(c[i]-target[i])<=tol for i in range(3))

def classify(c, name):
    r,g,b,a=c
    if a==0: return CLEAR
    if max(r,g,b) <= 24: return OUTLINE
    if near(c,(2,4,10),4) or near(c,(1,3,8),4): return OUTLINE
    # exact / near palette roles
    if near(c,(235,247,255),6): return WHITE
    if near(c,(166,255,242),6): return LIGHT
    if near(c,(26,143,134),8): return SHADOW
    if near(c,(77,255,224),8) or near(c,(148,255,238),8): return BODY
    if near(c,(255,140,46),10) or (r>210 and 75<g<190 and b<110): return ORANGE
    if near(c,(255,224,102),10) or (r>220 and g>185 and b<160): return YELLOW
    if near(c,(51,255,179),10) or near(c,(166,255,218),10): return GREEN
    if r>225 and g>230 and b>230: return WHITE
    # death / AA-less debris: map by luminance, keeping the ship hue family
    m=max(r,g,b)
    if m>225: return LIGHT
    if m>150: return BODY
    if m>75: return SHADOW
    return OUTLINE

def recolor_frame(im, name):
    out=Image.new('RGBA',(W,H),CLEAR); px=out.load()
    for y in range(H):
        for x in range(W):
            px[x,y]=classify(im.getpixel((x,y)),name)
    # Guarantee a 1px transparent canvas margin.
    for x in range(W):
        px[x,0]=CLEAR; px[x,H-1]=CLEAR
    for y in range(H):
        px[0,y]=CLEAR; px[W-1,y]=CLEAR
    return out

def interior(im):
    """Pixels with a non-outline colour."""
    return {(x,y) for y in range(H) for x in range(W)
            if im.getpixel((x,y))[3] and im.getpixel((x,y))[:3] != (2,4,10)}

def add_rim_light(im):
    px=im.load(); body=interior(im)
    for x,y in list(body):
        r,g,b,a=px[x,y]
        if (r,g,b)==BODY[:3]:
            top=px[x,y-1] if y>0 else CLEAR
            left=px[x-1,y] if x>0 else CLEAR
            if top[:3]==OUTLINE[:3] or left[:3]==OUTLINE[:3]:
                # only top-left hemisphere
                if y <= 16 or x <= 16:
                    px[x,y]=LIGHT
    return im

def add_wing_edge(im, accent):
    px=im.load()
    # Wings live outside the central body, where the old art used shadow pixels.
    for x in range(W):
        if 2 <= x <= 11 or 20 <= x <= 29:
            ys=[y for y in range(H) if px[x,y][3] and px[x,y][:3]==SHADOW[:3]]
            if ys:
                y=min(ys)
                px[x,y]=accent
    return im

def add_engine_core(im, accent):
    px=im.load()
    # Bottom-center engine block: swap dark/old pixels near the lower hull to accent.
    for y in range(24,H-1):
        for x in range(12,20):
            if px[x,y][3] and px[x,y][:3] in (BODY[:3],SHADOW[:3]):
                # keep a body frame around; central 4px becomes accent
                if 14 <= x <= 17 and 25 <= y <= 28:
                    px[x,y]=accent
                elif y==29 and 13 <= x <= 18:
                    px[x,y]=SHADOW
    return im

def add_cockpit_rim(im, accent):
    px=im.load()
    whites=[(x,y) for y in range(H) for x in range(W) if px[x,y][:3]==WHITE[:3]]
    if not whites: return im
    for x,y in whites:
        for dx,dy in ((1,0),(-1,0),(0,1),(0,-1)):
            nx,ny=x+dx,y+dy
            if 0<=nx<W and 0<=ny<H and px[nx,ny][3] and px[nx,ny][:3] in (BODY[:3],LIGHT[:3],SHADOW[:3]):
                px[nx,ny]=accent
    return im

def add_pulse(im, accent):
    px=im.load()
    # Small, deliberate energy pulse on the cockpit, engine and nose.
    for p in [(15,9),(16,9),(15,10),(16,10)]:
        if px[p][3] and px[p][:3] in (WHITE[:3],LIGHT[:3],BODY[:3]): px[p]=WHITE
    for y in range(25,29):
        for x in (14,15,16,17):
            if px[x,y][:3]==accent[:3]: px[x,y]=LIGHT
    return im

def clean_specks(im):
    """Remove 1px highlight glints that read as noise at gameplay scale."""
    px=im.load()
    for y in range(1,H-1):
        for x in range(1,W-1):
            c=px[x,y]
            if c[:3]!=LIGHT[:3]: continue
            neighbours=[px[x-1,y],px[x+1,y],px[x,y-1],px[x,y+1]]
            same=sum(1 for n in neighbours if n[:3] in (LIGHT[:3],WHITE[:3]))
            solid=sum(1 for n in neighbours if n[3] and n[:3] not in (LIGHT[:3],WHITE[:3]))
            if same==0:
                px[x,y]=BODY if solid else CLEAR
    return im

def make_hit(im):
    out=Image.new('RGBA',(W,H),CLEAR); px=out.load(); src=im.load()
    for y in range(H):
        for x in range(W):
            c=src[x,y]
            if c[3]==0: continue
            if c[:3]==OUTLINE[:3]: px[x,y]=OUTLINE
            else: px[x,y]=WHITE
    return out

def make_thruster(_frame, frame_index, accent):
    """Explicit 8x16 flame: accent body, light inner, white core, dark outline."""
    im=Image.new('RGBA',(8,16),CLEAR); px=im.load()
    lengths=[12,13,11]
    length=lengths[frame_index % 3]
    for i in range(length):
        y=i
        # Width tapers from 6 to 2.
        if i < 2: a,b=2,5
        elif i < 4: a,b=1,6
        elif i < 7: a,b=2,5
        elif i < 9: a,b=2,5
        else: a,b=3,4
        # Side flicker.
        if frame_index==1 and i in (3,4): b=6
        if frame_index==2 and i in (5,6): a=1
        for x in range(a,b+1):
            px[x,y]=accent
        # Dark outer edge.
        if b-a>=3:
            px[a,y]=OUTLINE; px[b,y]=OUTLINE
        # White hot core only near the nozzle.
        if 3 <= i <= 6:
            for x in (3,4):
                px[x,y]=WHITE
        elif i == 7:
            px[3,y]=LIGHT; px[4,y]=accent
        # Occasional side spark.
        if frame_index==2 and i==8:
            px[2,8]=LIGHT
    # Engine lip at the very top.
    for x in range(2,6):
        px[x,0]=OUTLINE if x in (2,5) else accent
    return im

def sheet(frames_, fw=W, fh=H):
    out=Image.new('RGBA',(fw*len(frames_),fh),CLEAR)
    for i,f in enumerate(frames_): out.alpha_composite(f,(i*fw,0))
    return out

def process(name):
    acc=SHIP_ACCENT[name]
    idle_old=frames(f'{ORIG}/ship_{name}_idle.png',32,32)
    death_old=frames(f'{ORIG}/ship_{name}_death.png',32,32)
    thr_old=frames(f'{ORIG}/ship_{name}_thruster.png',8,16)

    idle=[recolor_frame(f,name) for f in idle_old]
    for f in idle: add_rim_light(f)
    add_wing_edge(idle[0],acc); add_wing_edge(idle[1],acc)
    add_engine_core(idle[0],acc); add_engine_core(idle[1],acc)
    if name != 'baluarte':
        add_cockpit_rim(idle[0],acc); add_cockpit_rim(idle[1],acc)
    add_pulse(idle[1],acc)
    for f in idle: clean_specks(f)

    # Lanca: yellow nose/emitter and cockpit already become read through the accent rim.
    if name=='baluarte':
        # Small shoulder/lower armour accents; no rim around the cockpit.
        for f in idle:
            p=f.load()
            for x,y in [(7,10),(8,10),(9,10),(22,10),(23,10),(24,10)]:
                if p[x,y][3] and p[x,y][:3]==BODY[:3]: p[x,y]=LIGHT
            for x,y in [(8,18),(9,19),(10,20),(23,18),(22,19),(21,20)]:
                if p[x,y][3] and p[x,y][:3]==BODY[:3]: p[x,y]=SHADOW
            for x,y in [(3,12),(4,12),(27,12),(28,12)]:
                if p[x,y][3] and p[x,y][:3]==ORANGE[:3]: p[x,y]=LIGHT
            # Connect the side pods to the hull; they were floating one pixel away.
            for y in range(14,21):
                for x in (8,23):
                    if p[x,y][3]==0: p[x,y]=BODY
            for x,y in [(8,14),(23,14)]:
                if p[x,y][3]: p[x,y]=LIGHT
            for x,y in [(8,20),(23,20)]:
                if p[x,y][3]: p[x,y]=SHADOW
            # Heavier visible pod armour and shoulder plate band.
            for x in range(3,6):
                for y in range(14,20):
                    if p[x,y][3] and p[x,y][:3] in (BODY[:3],SHADOW[:3],LIGHT[:3]): p[x,y]=ORANGE
            for x in range(27,30):
                for y in range(14,20):
                    if p[x,y][3] and p[x,y][:3] in (BODY[:3],SHADOW[:3],LIGHT[:3]): p[x,y]=ORANGE
            for x,y in [(7,9),(8,9),(9,9),(10,9),(21,9),(22,9),(23,9),(24,9)]:
                if p[x,y][3] and p[x,y][:3]==BODY[:3]: p[x,y]=LIGHT
            for x,y in [(7,19),(8,19),(9,19),(10,19),(21,19),(22,19),(23,19),(24,19)]:
                if p[x,y][3] and p[x,y][:3]==BODY[:3]: p[x,y]=SHADOW
    if name=='lanca':
        for f in idle:
            p=f.load()
            # A deliberate 4px cannon barrel with a 2px core.
            for y in range(3,7):
                for x in (14,17):
                    if 0<=y<H: p[x,y]=OUTLINE
                for x in (15,16):
                    if 0<=y<H: p[x,y]=BODY
            for x in (14,17):
                if p[x,2][3]: p[x,2]=OUTLINE
        p=idle[0].load()
        for x in (15,16):
            for y in (1,2,3):
                if 0<=y<H: p[x,y]=YELLOW
        # 4px barrel: dark sides, cyan core, yellow emitter.
        for y in range(3,7):
            p[14,y]=OUTLINE; p[17,y]=OUTLINE
            p[15,y]=BODY; p[16,y]=BODY
        for x in (15,16):
            p[x,3]=YELLOW
        p=idle[1].load()
        for x in (15,16):
            for y in (1,2,3):
                if 0<=y<H: p[x,y]=WHITE
        for y in range(3,7):
            p[14,y]=OUTLINE; p[17,y]=OUTLINE
            p[15,y]=BODY; p[16,y]=BODY
        p=idle[1].load()
        for x in (15,16):
            for y in (1,2):
                if 0<=y<H: p[x,y]=WHITE

    hit=make_hit(idle[0])
    death=[]
    for f in death_old:
        r=recolor_frame(f,name)
        add_rim_light(r)
        death.append(r)

    thr=[make_thruster(f,i,acc) for i,f in enumerate(thr_old)]
    portrait=idle[0].resize((64,64),Image.NEAREST)

    sheet(idle).save(f'{OUT}/ship_{name}_idle.png')
    hit.save(f'{OUT}/ship_{name}_hit.png')
    sheet(death).save(f'{OUT}/ship_{name}_death.png')
    sheet(thr,8,16).save(f'{OUT}/ship_{name}_thruster.png')
    portrait.save(f'{OUT}/ui_pilot_{name}.png')
    # previews
    idle[0].resize((384,384),Image.NEAREST).save(f'{PREV}/{name}_12x.png')
    print('done',name)

if __name__=='__main__':
    os.makedirs(OUT,exist_ok=True); os.makedirs(PREV,exist_ok=True)
    for n in ['baluarte','vetor','lanca']: process(n)
