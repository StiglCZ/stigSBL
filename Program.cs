using System;
using System.IO;
using System.Collections.Generic;
class Program{

    public List<string> includes = new List<string>();
    public static Program program = new Program();
    public static void Main(string[] args) {
        string output = "";
        output =program.Full(args[0]);
        for(int i = 0; i < program.includes.Count; i++) {
            output = program.Full(program.includes[i]) + output;
        }
        output = "#include <stdio.h>\n#include <stdlib.h>\n" + output;
        File.WriteAllText("output.c",output);
    }
    public string[][] SplitString(string src){
        string[] commands = src.Replace("\t","").Replace("\n","").Split(';');
        List<string[]> vs = new List<string[]>();
        foreach (string v in commands) {
            vs.Add(v.Split(' '));
        }
        return vs.ToArray();
    }
    public string Full(string srcpath) {
        string src = File.ReadAllText(srcpath);
        string[][] code = program.SplitString(src);
        return program.Compile(code);
    }
    public string Compile(string[][] cmds) {
        string ccode = "";
        Dictionary<string, int> globvars = new Dictionary<string, int>();
        foreach (string[] cmd in cmds) {
            switch (cmd[0]) {
                case "incl":
                    includes.Add(cmd[1]);
                    break;
                case "call":
                    if (cmd.Length > 2) {
                        ccode += cmd[1] + "(" + cmd[2]+ ");";
                    } else {
                        ccode += cmd[1] + "();";
                    }
                    break;
                case "+":
                    ccode += cmd[3] + "=" +cmd[2] + "+" + cmd[1] +";";
                    break;
                case "-":
                    ccode += cmd[3] + "=" + cmd[1] + "-" + cmd[2] + ";";
                    break;
                case "*":
                    ccode += cmd[3] + "=" + cmd[2] + "*" + cmd[1] + ";";
                    break;
                case "/":
                    ccode += cmd[3] + "=" + cmd[1] + "/" + cmd[2] + ";";
                    break;
                case "func":
                    ccode += "int " + cmd[1] + "(";
                    if (cmd[1] != "main") {
                        ccode += "int arg){";
                    } else {
                        ccode += "){";
                    }
                    break;
                case "ret":
                    ccode += "return 0;";
                    break;
                case "end":
                    ccode += "return 0;}";
                    break;
                case "if":
                    ccode += "if(" + cmd[1] + cmd[2] + cmd[3] + "){" + cmd[4] + "(" + cmd[5]+ ");}";
                    break;
                case "var":
                    if (cmd[1] == "glob") {
                        int o;
                        if (int.TryParse(cmd[3], out o)) {
                            globvars.Add(cmd[2], int.Parse(cmd[3])); 
                         } else {
                            Console.WriteLine("Warning! global variable's " + cmd[2] + "assigning value isnt int");
                         }

                    } else {
                        ccode += "int " + cmd[2] +"=" + cmd[3] +";";
                    }
                    break;
                case "out":
                    if (cmd[1] == "str") {
                        ccode += "printf(\"" + cmd[2] + "\");";
                    } else if (cmd[1] == "int") {
                        ccode += "printf(\"%d\"," + cmd[2] + ");";
                    } else {
                        ccode += "printf(\"%c\"," + cmd[2] + ");";
                    }
                    break;
                case "in":
                    ccode += "scanf(\"%d\",&" + cmd[1] + ");";
                    break;
                case "space":
                    ccode += "printf(\" \");";
                    break;
                default :
                    ccode += "//Theres nothing!";
                    break;
            }
            ccode += "\n";
        }
        foreach (string globvar in globvars.Keys) {
            ccode = "int "+ globvar + "= " + globvars[globvar] + ";\n" + ccode;
        }
        return ccode;
    }
}
