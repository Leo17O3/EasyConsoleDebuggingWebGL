#if UNITY_WEBGL
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using System.IO;
using UnityEngine;

public class BuildChanger : IPostprocessBuildWithReport, IPreprocessBuildWithReport
{
    public int callbackOrder { get => 0; }

    private bool _isNeedToDebugLog;
    private bool _isNeedToDebugError;
    private bool _isNeedToDebugWarn;

    private const string _mainPartOfAdditiveText = @"
<!--Main part of additive text started-->
<img
      src=""./TemplateData/RoundButton.png""
      id = ""round-button""
      style=""
      position: fixed;
    max-width: 10%;
      height: auto;
      bottom: 10%;
      user-select:none;
      ""
      onclick=""show()""
    />
    <div
    id = ""console-catcher""
      style=""
        position: fixed;
    height: 25%;
        width: 75%;
        background-color: rgb(151, 202, 247);
    overflow-y: auto;
        line-break: anywhere;
        bottom: 15%;
        left: 12%;
        opacity: 0;
      ""
    ></div>
    <script>
      const consoleCatcher = document.getElementById(""console-catcher"");
    const roundButton = document.getElementById(""round-button"");

//console.log easy debug
//console.error easy debug
//console.warn easy debug

      let isHided = true;

    function show()
    {
        isHided = !isHided;
        consoleCatcher.style.opacity = isHided ? 0 : 1;

        rotate();
    }

    let currentDegrees = 0;

    function rotate()
    {
        currentDegrees = (currentDegrees + 180) % 360;
        roundButton.style.transform = `rotate(${ currentDegrees}
        deg)`;
    }
    </script>
<!--Main part of additive text finished-->";

    private const string _consoleLogAdditiveText = @"
const previousConsoleLog = console.log;

      console.log = function (...data) {
        for (let i = 0; i < data.length; i++) {
          consoleCatcher.innerHTML += `<span style=""color: rgb(116, 104, 104)"">${data[i]} </span>`;
        }

consoleCatcher.innerHTML += ""<br>"";
        previousConsoleLog(...data);
      };";

    private const string _consoleErrorAdditiveText = @"
const previousConsoleError = console.error;

      console.error = function (...data) {
        for (let i = 0; i < data.length; i++) {
          consoleCatcher.innerHTML += `<span style=""color: rgb(250, 37, 37)"">${data[i]} </span>`;
        }

consoleCatcher.innerHTML += ""<br>"";
        previousConsoleError(...data);
      };";

    private const string _consoleWarnAdditiveText = @"
const previousConsoleWarn = console.warn;

      console.warn = function (...data) {
        for (let i = 0; i < data.length; i++) {
          consoleCatcher.innerHTML += `<span style=""color: yellow"">${data[i]} </span>`;
        }

consoleCatcher.innerHTML += ""<br>"";
        previousConsoleWarn(...data);
      };";

    private string _tempRoundButtonPath = Path.GetDirectoryName(Application.dataPath) + "/RoundButton.png";

    public void OnPreprocessBuild(BuildReport report)
    {
        File.Move(Application.dataPath + "/EasyConsoleDebuggingWebGL/Editor/Images/RoundButton.png", _tempRoundButtonPath);
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        File.Move(_tempRoundButtonPath, Application.dataPath + "/EasyConsoleDebuggingWebGL/Editor/Images/RoundButton.png");

        _isNeedToDebugLog = EditorPrefs.GetBool("IsNeedToDebugLog", false);
        _isNeedToDebugError = EditorPrefs.GetBool("IsNeedToDebugError", false);
        _isNeedToDebugWarn = EditorPrefs.GetBool("IsNeedToDebugWarn", false);

        string html = File.ReadAllText(report.summary.outputPath + "/index.html");
        int previousHtmlHashCode = html.GetHashCode();

        bool isHtmlContainsMainPart = html.Contains("Main part of additive text started");
        bool isHtmlContainsConsoleLogPart = html.Contains("const previousConsoleLog = console.log;");
        bool isHtmlContainsConsoleErrorPart = html.Contains("const previousConsoleError = console.error;");
        bool isHtmlContainsConsoleWarnPart = html.Contains("const previousConsoleWarn = console.warn;");

        if (isHtmlContainsMainPart &&
            (!_isNeedToDebugLog && isHtmlContainsConsoleLogPart ||
            (!_isNeedToDebugError && isHtmlContainsConsoleErrorPart) ||
            (!_isNeedToDebugWarn && isHtmlContainsConsoleWarnPart) ||
            (!IsNeedToCreateChanges()))
            )
        {
            isHtmlContainsMainPart = false;
            isHtmlContainsConsoleLogPart = false;
            isHtmlContainsConsoleErrorPart = false;
            isHtmlContainsConsoleWarnPart = false;

            html = html.Remove(html.IndexOf("<!--Main part of additive text started-->") - 2, html.IndexOf("Main part of additive text finished") - html.IndexOf("Main part of additive text started") + 44);

            if (!IsNeedToCreateChanges())
            {
                File.WriteAllText(report.summary.outputPath + "/index.html", html);

                if (File.Exists(report.summary.outputPath + "/TemplateData/RoundButton.png"))
                    File.Delete(report.summary.outputPath + "/TemplateData/RoundButton.png");

                return;
            }
        }
        if (!isHtmlContainsMainPart)
            html = html.Insert(html.IndexOf("</body>") - 1, _mainPartOfAdditiveText);

        if (!isHtmlContainsConsoleLogPart)
            html = html.Insert(html.IndexOf("//console.log easy debug"), _isNeedToDebugLog ? _consoleLogAdditiveText : "");

        if (!isHtmlContainsConsoleErrorPart)
            html = html.Insert(html.IndexOf("//console.error easy debug"), _isNeedToDebugError ? _consoleErrorAdditiveText : "");

        if (!isHtmlContainsConsoleWarnPart)
            html = html.Insert(html.IndexOf("//console.warn easy debug"), _isNeedToDebugWarn ? _consoleWarnAdditiveText : "");

        if (html.GetHashCode() != previousHtmlHashCode)
            File.WriteAllText(report.summary.outputPath + "/index.html", html);

        if (!File.Exists(report.summary.outputPath + "/TemplateData/RoundButton.png"))
            File.Copy(Application.dataPath + "/EasyConsoleDebuggingWebGL/Editor/Images/RoundButton.png", report.summary.outputPath + "/TemplateData/RoundButton.png");
    }

    private bool IsNeedToCreateChanges()
    {
        return _isNeedToDebugLog || _isNeedToDebugError || _isNeedToDebugWarn;
    }
}
#endif