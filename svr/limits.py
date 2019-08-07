from sympy import symbols, limit
from function_parser import buildFunctionFromTree, parseToTree, getConstant

def limitExpression(txt_expression, txt_variables, txt_argument, txt_value, txt_dir):
    try:
        sympy_variables = {}

        for var in txt_variables:
            sympy_variables[var] = symbols(var)

        sympy_function = buildFunctionFromTree(parseToTree(txt_expression), sympy_variables)
        sympy_argument = sympy_variables[txt_argument]
        sympy_value = getConstant(txt_value)
        if sympy_value == None:
            sympy_value = sympy_variables[txt_value]

        result = limit(sympy_function, sympy_argument, sympy_value, txt_dir)
        return [True, str(result)]
    except:
        return [False]
