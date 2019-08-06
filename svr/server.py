import http.server
import socketserver
import sympy
import base64
from urllib.parse import urlparse, parse_qs
from integration import integrate
from limits import limit

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
        if len(params["method"]) == 1:
            if params["method"][0] == "int": 
                # Parameter: function (base64 repräsentation des Präfixterms), vars (variablen), lim1&lim2 (obere&untere grenze), delta (Variable nach der integriert wird)
                function_text = base64.b64decode((params["function"][0]).encode()).decode("utf-8") 
                variables = params["vars"]
                delta = params["delta"][0]
                lim = None
                if params.__contains__("lim1") and params.__contains__("lim2"):
                    lim = (params["lim1"][0], params["lim2"][0])
                return integrate(function_text, variables, delta, lim)
            elif params["method"][0] == "lim":
                function_text = base64.b64decode((params["function"][0]).encode()).decode("utf-8") 
                variables = params["vars"]
                argument_text = params["arg"][0]
                lim = params["lim"][0]
                if params["dir"][0] == "1":
                    dir = "+"
                elif params["dir"][0] == "2":
                    dir = "-"
                elif params["dir"][0] == "3":
                    dir = "+-"
                return limit(function_text, variables, argument_text, lim, dir)
    return "false"
   
with socketserver.TCPServer(("", PORT), SimpleHTTPRequestHandler) as httpd:
    print("serving at port", PORT)
    httpd.serve_forever()
