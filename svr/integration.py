from sympy import symbols, integrate
from function_parser import buildFunctionFromTree, parseToTree, getConstant

def integrateExpression(txt_expression, txt_variables, txt_delta, txt_limit):
    try:
        sympy_variables = {}

        for var in txt_variables:
            sympy_variables[var] = symbols(var)

        sympy_expression = buildFunctionFromTree(parseToTree(txt_expression), sympy_variables)
        delta = sympy_variables[txt_delta]

        if txt_limit == None:
            result = integrate(sympy_expression, delta)
        else:
            lower_limit = buildFunctionFromTree(parseToTree(txt_limit[0]), sympy_variables)
            upper_limit = buildFunctionFromTree(parseToTree(txt_limit[1]), sympy_variables)
            result = integrate(sympy_expression, (delta, lower_limit, upper_limit))
        
        return [True, str(result)]
    except:
        return [False]
        
