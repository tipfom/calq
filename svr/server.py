import http.server
import socketserver
import sympy
import base64
from urllib.parse import urlparse, parse_qs
from integration import integrateExpression
from limits import limitExpression
from solver import solveExpression
import threading

PORT = 8080

version = "1.0"

class SimpleHTTPRequestHandler(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == "/handshake":
            self.handleGET_Handshake()
        elif self.path.startswith("/math"):
            self.handleGET_Math(parse_qs(urlparse(self.path).query))
        else:
            self.send_response(200)
            self.end_headers()
            self.wfile.write(threading.current_thread().getName().encode())
    
      
    def replyError(self, error_message):
        self.send_response(400)
        self.end_headers()
        self.wfile.write(error_message.encode())
        
    def handleGET_Handshake(self):
        self.send_response(200)
        self.end_headers()
        self.wfile.write(version.encode())

    def replyResult(self, r):
        if r[0]:
            self.send_response(200)
            self.end_headers()
            self.wfile.write(r[1].encode())
        else:
            self.replyError("error on limit")


    def handleGET_Math(self, params):
        if params.__contains__("method") and len(params["method"]) == 1:
            if params["method"][0] == "int": 
                if not params.__contains__("expr") or len(params["expr"]) != 1 or not params.__contains__("var") or not params.__contains__("d") or len(params["d"]) != 1:
                    return self.replyError("invalid parameters for integrate")
        
                txt_expression = base64.b64decode((params["expr"][0]).encode()).decode("utf-8") 
                txt_variables = params["var"]
                txt_delta = params["d"][0]
                txt_limit = None
                if params.__contains__("llim") and params.__contains__("ulim"):
                    txt_limit = (params["llim"][0], params["ulim"][0])
                self.replyResult(integrateExpression(txt_expression, txt_variables, txt_delta, txt_limit))

            elif params["method"][0] == "lim":
                if not params.__contains__("expr") or len(params["expr"]) != 1 or not params.__contains__("var") or not params.__contains__("arg") or len(params["arg"]) != 1 or not params.__contains__("val") or len(params["val"]) != 1:
                    return self.replyError("invalid parameters for limit")
                
                txt_expression = base64.b64decode((params["expr"][0]).encode()).decode("utf-8") 
                txt_variables = params["var"]
                txt_argument = params["arg"][0]
                txt_value = params["val"][0]
                if params["dir"][0] == "1":
                    txt_dir = "+"
                elif params["dir"][0] == "2":
                    txt_dir = "-"
                elif params["dir"][0] == "3":
                    txt_dir = "+-"
                self.replyResult(limitExpression(txt_expression, txt_variables, txt_argument, txt_value, txt_dir))
    
            elif params["method"][0] == "sol":
                if not params.__contains__("expr")  or not params.__contains__("var") or not params.__contains__("arg") or len(params["arg"]) != 1 or not params.__contains__("val") or len(params["val"]) != 1 or not params.__contains__("solve"):
                    return self.replyError("invalid parameters for limit")
                
                txts_expression = []
                for func in params["function"]:
                    txts_expression.append(base64.b64decode(func.encode()).decode("utf-8"))
                txt_variables = params["var"]
                txt_solve = params["solve"]
                self.replyResult(solveExpression(txts_expression, txt_variables, txt_solve))

            else:
                return self.replyError("method unknown")
        else:
            return self.replyError("method missing")
   
import sys, os, socket
from socketserver import ThreadingMixIn
from http.server import HTTPServer

class ThreadingSimpleServer(ThreadingMixIn, HTTPServer):
    pass

server = ThreadingSimpleServer(("", PORT), SimpleHTTPRequestHandler)
while 1:
    server.handle_request()