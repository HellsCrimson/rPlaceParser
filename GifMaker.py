import glob
import os
from PIL import Image


def make_gif(frame_folder):
    frames = []
    files = []
    for image in glob.glob(f"{frame_folder}*.bmp"):
        files.append(image)
    files.sort()
    for file in files:
        frames.append(Image.open(file))
    frame_one = frames[0]
    frame_one.save("HeatMap.gif", format="GIF", append_images=frames,
               save_all=True, duration=40, loop=0)
    

if __name__ == "__main__":
    make_gif("./Resources/toGif/")
    os.system('ffmpeg -i HeatMap.gif -movflags faststart -pix_fmt yuv420p -vf "scale=trunc(iw/2)*2:trunc(ih/2)*2" HeatMap.mp4')