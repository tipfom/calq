from sympy import symbols, solve
from function_parser import buildFunctionFromTree, parseToTree

def solveExpression(txts_expression, txts_variables, txts_solve):
    try:
        sympy_variables = {}

        for var in txts_variables:
            sympy_variables[var] = symbols(var)

        sympy_expression = []
        for txt_expression in txts_expression:
            sympy_expression.append(buildFunctionFromTree(parseToTree(txt_expression), sympy_variables))

        solve_for = []
        for var in txts_solve:    
            solve_for.append(sympy_variables[var])

        sol = solve(sympy_expression, solve_for)
        return [True, str(sol)]
    except:
        return [False]