import http.server
import socketserver
import sympy
import base64
from urllib.parse import urlparse, parse_qs
from integration import integrate
from parse_function import *

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
        if(len(params["method"]) == 1 and params["method"][0] == "int"):
            # function=base64
            # vars=list
            # lim1, lim2
            function_text = base64.b64decode((params["function"][0]).encode()).decode("utf-8") 
            variables = params["vars"]
            lim = None
            if params.__contains__("lim1") and params.__contains__("lim2"):
                lim = (params["lim1"][0], params["lim2"][0])
            return integrate(function_text, variables, lim)
    return "false"
   
with socketserver.TCPServer(("", PORT), SimpleHTTPRequestHandler) as httpd:
    print("serving at port", PORT)
    httpd.serve_forever()
