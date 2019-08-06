from sympy import symbols
from function_parser import *

def integrate(funcion_text, variables, delta_text, lim = None):
    sympy_variables = {}

    for var in variables:
        sympy_variables[var] = symbols(var)

    sympy_function = buildFunctionFromTree(parseToTree(funcion_text), sympy_variables)
    delta = sympy_variables[delta_text]

    if lim == None:
        integral = sympy.integrate(sympy_function, delta)
    else:
        lower_limit = getConstant(lim[0])
        if lower_limit == None:
            lower_limit = sympy_variables[lim[0]]

        upper_limit = getConstant(lim[1])
        if upper_limit == None:
            upper_limit = sympy_variables[lim[1]]
        integral = sympy.integrate(sympy_function, (delta, lower_limit, upper_limit))
    return str(integral)
