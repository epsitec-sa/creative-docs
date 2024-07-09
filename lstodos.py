import re
import argparse
from pathlib import Path
from collections import defaultdict, Counter

PROJECT_ROOT = "./"
FOLDERS = [
    "App.CreativeDocs",
    "Common",
    "Common.Document",
    "Common.DocumentEditor",
    "Common.Tests",
]


TODO_STRING = r"bl-net8-cross *(\w+)?"
NOT_IMPLEMENTED_STRING = r"throw new .*NotImplementedException"
IGNOREFILE_STRING = r"IGNOREFILE"

RED = 31
GREEN = 32
YELLOW = 33
BRIGHT_YELLOW = 93
PURPLE = 35
CYAN = 36
BLUE = 94

TODO_DISPLAY = {
    "default": ("*", CYAN),
    "important": ("/!\\", BRIGHT_YELLOW),
    "clipboard": ("c", YELLOW),
    "printing": ("p", YELLOW),
    "notimplemented": ("x", RED),
    "maybedelete": ("#", PURPLE),
    "cleanup": ("~", PURPLE),
}

DEFAULT_CATEGORIES = ["default", "important", "notimplemented", "maybedelete"]

def color(text, color):
    return f"\033[{color}m{text}\033[0m"

def extract_todos(file, autoignore):
    with open(file, encoding='utf-8') as f:
        content = f.read()
    if autoignore and IGNOREFILE_STRING in content:
        print(f"Ignore file {file}")
        return
    for todo in re.finditer(TODO_STRING, content):
        yield todo.group(1) if todo.group(1) in TODO_DISPLAY.keys() else "default"
    for _ in re.finditer(NOT_IMPLEMENTED_STRING, content):
        yield "notimplemented"

def enum_todos_by_file(todo_filter, autoignore):
    for folder in FOLDERS:
        folder_root = Path(PROJECT_ROOT) / folder
        for file in folder_root.rglob("*.cs"):
            todos = todo_filter(list(extract_todos(file, autoignore)))
            if todos:
                yield (file.parts, todos)

def make_todo_tree(todos_by_file):
    folders = defaultdict(list)
    files = {}
    for (first, *remain), todos in todos_by_file:
        if remain:
            folders[first].append((remain, todos))
        else:
            files[first] = todos
    folders = {name: make_todo_tree(content) for name, content in folders.items()}
    return folders, files

def get_todo_count(tree):
    folders, files = tree
    counter = Counter()
    for todos in files.values():
        counter += Counter(todos)
    if folders:
        for f in folders.values():
            counter += get_todo_count(f)
    return counter

def print_tree(tree, depth=0):
    folders, files = tree
    indent = "|   "*depth
    for folder, content in sorted(folders.items()):
        todo_count = get_todo_count(content).total()
        if todo_count == 0:
            continue
        print(f"{indent}{color(folder, BLUE)}/ ({todo_count})")
        print_tree(content, depth+1)
    align_column = max(len(f) for f in files.keys()) if files else 0
    for file, todos in sorted(files.items()):
        filename = file.ljust(align_column)
        todo_count = len(todos)
        todo_line = "".join([color(*TODO_DISPLAY.get(t, TODO_DISPLAY["default"])) for t in todos])
        print(f"{indent}{color(filename, GREEN)} ({todo_count}) {color(todo_line, RED)}")

def filter_tree(tree, search):
    folders, files = tree
    folders = {
        name: content if search.lower() in name.lower() else filter_tree(content, search)
        for name, content in folders.items()
    }
    files = {
        name: todos
        for name, todos in files.items() if search in name.lower()
    }
    return folders, files

def count_files(tree):
    folders, files = tree
    return sum(count_files(f) for f in folders.values()) + len(files)

def make_todo_filter(categories_desc):
    if categories_desc is None:
        filtered_categories = DEFAULT_CATEGORIES
    elif categories_desc == "all":
        return lambda x: x
    else:
        filtered_categories = [c.strip() for c in categories_desc.split(",")]
    def todo_filter(todos):
        return [t for t in todos if t in filtered_categories]
    return todo_filter


if __name__ == "__main__":
    parser = argparse.ArgumentParser(
        prog="lstodos",
        description="list todos in CreativeDocs",
    )
    parser.add_argument("-s", "--search",
                        help="display only some part of the tree")

    parser.add_argument("-c", "--category",
                        help="display only some categories")

    parser.add_argument("--show-ignored",
                        help=f"display ignored file (containing the magic string {IGNOREFILE_STRING}",
                        action="store_true",
                        )
    args = parser.parse_args()

    todos_by_file = list(enum_todos_by_file(make_todo_filter(args.category), not args.show_ignored))
    tree = make_todo_tree(todos_by_file)
    if args.search:
        tree = filter_tree(tree, args.search)
    print()
    print("Todo categories:")
    count_by_category = get_todo_count(tree)
    for category, (char, c) in TODO_DISPLAY.items():
        count = count_by_category.get(category, 0)
        print(color(f"    {char} {category} ({count})", c))
    print()
    files_count = count_files(tree)
    print(f"There are {files_count} files matching your request.")
    print()
    print_tree(tree)
