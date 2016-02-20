//  Copyright (c) 2016, HaeriStudios
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:
//  
//  1. Redistributions of source code must retain the above copyright notice, 
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions in binary form must reproduce the above copyright notice, 
//     this list of conditions and the following disclaimer in the documentation 
//     and/or other materials provided with the distribution.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
//  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//  MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL 
//  THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
//  OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof(Text))]
public class DBugger : MonoBehaviour
{
    public bool isEnabled = true;
    public bool showScriptNames = true;
    public int floatPrecision = 2;
    public int vectorPrecision = 2;
    public Color textColor = Color.black;

    private Text debugText;

    private static string fPrecision;
    private static string vPrecision;
    private static string[] TextArray;
    private static LinkedList<Object> clients = new LinkedList<Object>();
    private static int id;
    private static bool isReady = false;
    private static bool staticEnable;

    private const string COLOR_RED = "#ff0000";
    private const string COLOR_GREEN = "#00ff00";
    private const string COLOR_BLUE = "#0000ff";

    // Init Components
    void Start()
    {
        debugText = this.GetComponent<Text>();
        StartCoroutine(waitForInit());
        debugText.supportRichText = true;
        debugText.rectTransform.anchorMin = new Vector2(0, 0);
        debugText.rectTransform.anchorMax = new Vector2(1,1);
        debugText.rectTransform.offsetMin = new Vector2(debugText.rectTransform.offsetMin.x, 10);
        debugText.rectTransform.offsetMax = new Vector2(debugText.rectTransform.offsetMax.x, -10);
        debugText.rectTransform.offsetMin = new Vector2(debugText.rectTransform.offsetMin.y, 0);
        debugText.rectTransform.offsetMax = new Vector2(debugText.rectTransform.offsetMax.y, 0);
    }

    // Update Debug Text
    void LateUpdate()
    {
        if (isEnabled)
        {
            debugText.enabled = true;
            staticEnable = true;
        }
        else
        {
            debugText.enabled = false;
            staticEnable = false;
            return;
        }

        if (TextArray != null && isReady)
        {
            string temp = "";
            for (int i = 0; i < id; i++)
            {
                if(showScriptNames)
                    temp += "[" + clients.ElementAt(i).GetType() + "]\t" + TextArray[i] + "\n";
                else
                    temp += TextArray[i] + "\n";
                TextArray[i] = "";
            }
            debugText.text = temp;
        }

        debugText.color = textColor;
        fPrecision = setPrecision(floatPrecision);
        vPrecision = setPrecision(vectorPrecision);
    }

    /// <summary>
    /// Creates a new line. Following log calls are going to appear on the next line.
    /// </summary>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void newline(int id)
    {
        if (isReady && staticEnable)
        {
            TextArray[id] += "\n";
        }
    }

    /// <summary>
    /// Log a message.
    /// </summary>
    /// <param name="message">message to log</param>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void log(string message, int id)
    {
        if (isReady && staticEnable)
        {
            TextArray[id] += message + "\t";
        }
    }

    /// <summary>
    /// Log a message and set font color.
    /// </summary>
    /// <param name="message">message to log</param>
    /// <param name="color">color of log text</param>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void log(string message, Color color, int id)
    {
        log("<Color=" + hexConverter(color) + ">" + message + "</Color>", id);
    }

    /// <summary>
    /// Log a boolean.
    /// </summary>
    /// <param name="name">name of the boolean</param>
    /// <param name="boolean">boolean to log</param>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void log(string name, bool boolean, int id)
    {
        if(boolean)
            log(name + ": <Color=" + COLOR_GREEN + ">" + boolean + "</Color>", id);
        else
            log(name + ": <Color=" + COLOR_RED + ">" + boolean + "</Color>", id);
    }

    /// <summary>
    /// Log a vector2.
    /// </summary>
    /// <param name="name">name of the vector</param>
    /// <param name="vector">vector to log</param>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void log(string name, Vector2 vector, int id)
    {
        log(name + ": (<Color=" + COLOR_RED + ">" + vector.x.ToString(vPrecision) + "</Color>,<Color=" + COLOR_GREEN + ">" + vector.y.ToString(vPrecision) + "</Color>)", id);
    }

    /// <summary>
    /// Log a vector3.
    /// </summary>
    /// <param name="name">name of the vector</param>
    /// <param name="vector">vector to log</param>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void log(string name, Vector3 vector, int id)
    {
        log(name + ": (<Color=" + COLOR_RED + ">" + vector.x.ToString(vPrecision) + "</Color>,<Color=" + COLOR_GREEN + ">" + vector.y.ToString(vPrecision) + "</Color>,<Color=" + COLOR_BLUE + ">" + vector.z.ToString(vPrecision) + "</Color>)", id);
    }

    /// <summary>
    /// Log a float.
    /// </summary>
    /// <param name="name">name of the float</param>
    /// <param name="_float">float to log</param>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void log(string name, float _float, int id)
    {
        log(name + ": " + _float.ToString(fPrecision), id);
    }

    /// <summary>
    /// Log a variable.
    /// </summary>
    /// <param name="name">name of the variable</param>
    /// <param name="var">variable to log</param>
    /// <param name="id">Unique script ID. Use requestID() to get the ID</param>
    public static void log(string name, string var, int id)
    {
        log(name + ": " + var, id);
    }

    /// <summary>
    /// Request a unique ID to access the log methode.
    /// IMPORTANT: ONLY CALL ONCE PER SCRIPT!
    /// </summary>
    /// <param name="client">script reference</param>
    /// <return>Returns the unique ID</return>
    public static int requestID(Object client)
    {
        if (isReady)
            TextArray = new string[id + 1];
        clients.AddLast(client);

        return id++;
    }



    // Wait untill all scripts have rquested an Id
    IEnumerator waitForInit()
    {
        yield return new WaitForEndOfFrame();
        TextArray = new string[id];
        isReady = true;
    }

    // Convert from Color to hex
    private static string hexConverter(Color color)
    {
        Color myColor = color;
        int r = (int)myColor.r * 255;
        int g = (int)myColor.g * 255;
        int b = (int)myColor.b * 255;
        string temp = r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        return "#" + temp;
    }

    // Returns a string that can be used for float rounding
    private string setPrecision(int _precision)
    {
        if (_precision == 0) 
            return "0";

        string temp = "0.";
        for (int i = 0; i < _precision; i++)
            temp += "0";

        return temp;
    }
}