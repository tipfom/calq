from function_parser import buildFunctionFromTree, parseToTree
from sympy import symbols, preview, init_printing
from pathlib import Path

import os
os.environ["PATH"] += os.pathsep + 'E:\\Programme\\MikTex\\miktex\\bin\\x64'


c = 1
init_printing(use_latex="mathjax")


def getDrawnLatex(txt_expression):
    global c
    c += 1
    out = str(c) + ".png"
    preview(txt_expression, viewer='file', filename=out, dvioptions=[
            "-T", "tight", "-z", "0", "-D", "500", "--truecolor"])
    return out


preview("{({x}^{2})}", viewer='file', filename="t.png", dvioptions=[
    "-T", "tight", "-z", "0", "-D", "500", "--truecolor"])
