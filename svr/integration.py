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
            lower_limit = getConstant(txt_limit[0])
            if lower_limit == None:
                lower_limit = sympy_variables[txt_limit[0]]

            upper_limit = getConstant(txt_limit[1])
            if upper_limit == None:
                upper_limit = sympy_variables[txt_limit[1]]
            result = integrate(sympy_expression, (delta, lower_limit, upper_limit))
        
        return [True, str(result)]
    except:
        return [False]
        
