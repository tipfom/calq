from parse_function import *
from sympy import symbols

def integrate(func_str, vars, lim = None):
    sympyVars = {}

    for var in vars:
        sympyVars[var] = symbols(var)

    func = buildFunctionFromTree(parseToTree(func_str), sympyVars)
    delta = sympyVars["x"]

    if lim == None:
        integral = sympy.integrate(func, delta)
    else:
        lower_limit_node = Node(lim[0], 0,0,[])
        if lower_limit_node.isConstant():
            lower_limit = lower_limit_node.const
        else:
            lower_limit = sympyVars[lower_limit_node.key]
        upper_limit_node = Node(lim[1], 0,0,[])
        if upper_limit_node.isConstant():
            upper_limit = upper_limit_node.const
        else:
            upper_limit = sympyVars[upper_limit_node.key]
        integral = sympy.integrate(func, (delta, lower_limit, upper_limit))
    return str(integral)
