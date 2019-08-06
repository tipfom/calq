import http.server
import socketserver
import sympy
import base64
from urllib.parse import urlparse, parse_qs
from sympy import symbols
from mpmath import pi as const_pi, e as const_euler

PORT = 8080


class SimpleHTTPRequestHandler(http.server.BaseHTTPRequestHandler):

    def do_GET(self):
        self.send_response(200)
        self.send_header('Content-type', 'text/html')
        self.end_headers()
        parsedUrl = parse_qs(urlparse(self.path).query)
        self.wfile.write(execCommand(parsedUrl).encode())
        return


def execCommand(params):
    if params.__contains__("method"):
        if(len(params["method"]) == 1 and params["method"][0] == "int"):
            # function=base64
            # vars=list
            func = base64.b64decode((params["function"][0]).encode()).decode("utf-8") 
            ar = params["vars"]
            return integrate(func, ar)
    return "false"

class Node():
    def __init__(self, key, start, end, childs):
        self.key=key
        self.start=start
        self.end=end
        self.childs=childs

    def isVariable(self):
        return len(self.childs) == 0
    
    def isConstant(self):
        if (self.isVariable()):
            if self.key == "pi":
                self.const = const_pi
                return True
            elif self.key == "e":
                self.const = const_euler
                return True
            else:
                try:
                    self.const = int(self.key)
                    return True
                except:
                    return False

        return False
    

def parseToTree(function, start=0):
    k = ""
    i = start
    while i < len(function):
        if function[i] == "[":
            # we need to go deeper
            looping = True
            inners = []

            while looping:
                inner = parseToTree(function, i+1)
                inners.append(inner)
                if (inner.end < len(function) and function[inner.end] == ","):
                    i=inner.end
                else:
                    looping = False

            return Node(k, start, inner.end+1, inners)

        if function[i] == "]":
            return Node(k, start, i, [])

        if function[i] == ",":
            return Node(k, start, i, [])

        k += function[i]

        i += 1
        if i == len(function):
            break
    return Node(k, start, len(function), [])


def buildFunctionFromTree(tree, sympy_vars):
    expr = 0

    if tree.isConstant():
        expr = tree.const

    elif tree.isVariable():
        expr = sympy_vars[tree.key]

    elif tree.key == "+":
        for ch in tree.childs:
            expr += buildFunctionFromTree(ch, sympy_vars)
    elif tree.key == "-":
        for ch in tree.childs:
            expr -= buildFunctionFromTree(ch, sympy_vars)
    
    elif tree.key == "*":
        for ch in tree.childs:
            expr *= buildFunctionFromTree(ch, sympy_vars)
    
    elif tree.key == "/":
        expr = buildFunctionFromTree(tree.childs[0], sympy_vars) / buildFunctionFromTree(tree.childs[1], sympy_vars)

    elif tree.key == "sin":
        expr = sympy.sin(buildFunctionFromTree(tree.childs[0], sympy_vars))

    elif tree.key == "cos":
        expr = sympy.cos(buildFunctionFromTree(tree.childs[0], sympy_vars))

    elif tree.key == "ln":
        expr = sympy.ln(buildFunctionFromTree(tree.childs[0], sympy_vars))

    elif tree.key == "^":
        expr = buildFunctionFromTree(tree.childs[0], sympy_vars) ** buildFunctionFromTree(tree.childs[1], sympy_vars)

    return expr


def integrate(func_str, vars):
    sympyVars = {}

    for var in vars:
        sympyVars[var] = symbols(var)

    func = buildFunctionFromTree(parseToTree(func_str), sympyVars)
    delta = sympyVars["x"]

    integral = sympy.integrate(func, delta)

    return str(integral)

x = integrate("+[/[1,x],ln[^[e,x]]]", ["x"])

with socketserver.TCPServer(("", PORT), SimpleHTTPRequestHandler) as httpd:
    print("serving at port", PORT)
    httpd.serve_forever()

