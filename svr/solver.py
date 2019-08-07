from sympy import symbols, solve
from function_parser import buildFunctionFromTree, parseToTree

def execSolve(function_texts, variables, solve_for_texts):
    sympy_variables = {}

    for var in variables:
        sympy_variables[var] = symbols(var)

    functions = []
    for function_text in function_texts:
        functions.append(buildFunctionFromTree(parseToTree(function_text), sympy_variables))

    solve_for = []
    for var in solve_for_texts:    
        solve_for.append(sympy_variables[var])

    sol = solve(functions, solve_for)
    return str(sol)