from function_parser import *
from sympy import symbols

def limit(funcion_text, variables, argument_text, lim, dir):
    sympy_variables = {}

    for var in variables:
        sympy_variables[var] = symbols(var)

    sympy_function = buildFunctionFromTree(parseToTree(funcion_text), sympy_variables)
    sympy_argument = sympy_variables[argument_text]
    sympy_limit = getConstant(lim)
    if sympy_limit == None:
        sympy_limit = sympy_variables[lim]

    limit = sympy.limit(sympy_function, sympy_argument, sympy_limit, dir)
    return str(limit)
