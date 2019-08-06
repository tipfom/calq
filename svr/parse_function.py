import sympy
from sympy import symbols, oo

const_pi = sympy.pi
const_e = sympy.exp(1)

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
                self.const = const_e
                return True

            elif self.key == "pinf":
                self.const = +oo
                return True

            elif self.key == "ninf":
                self.const = -oo
                return True

            else:
                try:
                    self.const = int(self.key)
                    return True
                except:
                    return False

        return False