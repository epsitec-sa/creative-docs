from pathlib import Path
import shutil

for file in Path(".").glob("*.xml"):
    dest = file.stem + ".icon"
    shutil.move(file, dest)
