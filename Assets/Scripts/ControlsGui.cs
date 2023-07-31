#region

using UnityEngine;

#endregion

public class ControlsGui : MonoBehaviour
{
    private bool _windowOpen;

    private Rect _windowRect = new(Screen.width - 520, Screen.height - 320, 300, 600);

    private void DoMyWindow(int windowId)
    {
        const int offsetX = 10;
        var offsetY = 30;

        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Rotate Model");
        offsetY += 20;
        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Right Mouse Button");
        offsetY += 30;

        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Move Model");
        offsetY += 20;
        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Middle Mouse Button");
        offsetY += 30;

        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Zoom In/Out");
        offsetY += 20;
        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Mouse Scroll Wheel");
        offsetY += 30;

        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Rotate Light");
        offsetY += 20;
        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Middle Mouse Button + L");
        offsetY += 30;

        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Rotate Background");
        offsetY += 20;
        GUI.Label(new Rect(offsetX, offsetY, 250, 30), "Middle Mouse Button + B");
        offsetY += 30;

        if (GUI.Button(new Rect(offsetX + 160, offsetY, 120, 30), "Close")) _windowOpen = false;
    }

    private void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(MainGui.Instance.guiScale.x, MainGui.Instance.guiScale.y, 1));

        _windowRect = new Rect((Screen.width / MainGui.Instance.guiScale.x) - 480, (Screen.height / MainGui.Instance.guiScale.y) - 370, 170, 280);

        if (_windowOpen) _windowRect = GUI.Window(22, _windowRect, DoMyWindow, "Controls");

        if (!GUI.Button(new Rect((Screen.width / MainGui.Instance.guiScale.x) - 370, (Screen.height / MainGui.Instance.guiScale.y) - 40, 80, 30), "Controls")) return;
        _windowOpen = !_windowOpen;
    }
}