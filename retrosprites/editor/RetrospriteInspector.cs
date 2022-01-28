using System.Collections;
using System.Collections.Generic;
//using System;
//using System.Reflection;
using UnityEngine;
using UnityEditor;

public class RetrospriteInspector : ShaderGUI
{
  Material target;
  MaterialEditor editor;
  MaterialProperty[] properties;
  string[] textureTypes = new string[] { "Array of individual sprites", "Array of sprite sheets"};
  int[] textureTypeVal = new int[] { 0, 1 };
  public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
  {
    this.target = editor.target as Material;
    this.editor = editor;
    this.properties = properties;
    DrawGUI();
  }

  private void DrawGUI()
  {
    MaterialProperty mainTex = FindProperty("_MainTex", properties);
    MaterialProperty color = FindProperty("_Color", properties);
    MaterialProperty light = FindProperty("_light", properties);
    MaterialProperty alphaClip = FindProperty("_alphaClip", properties, false);
    MaterialProperty dir = FindProperty("_Dir", properties);
    MaterialProperty invRot = FindProperty("_InvRot", properties);
    MaterialProperty frame = FindProperty("_frame", properties);
    MaterialProperty params1 = FindProperty("_Params", properties, false);

    GUIContent mainTexLabel = new GUIContent(mainTex.displayName, "Collection of sprite textures assembled into a Texture2DArray asset");
    GUIContent colorLabel = new GUIContent(color.displayName, "RGBA color to multiply the sprite's color by");
    GUIContent lightLabel = new GUIContent(light.displayName, "Apply lighting from the world or be unlit");
    GUIContent alphaClipLabel = new GUIContent(alphaClip != null ? alphaClip.displayName : "", "Alpha threshold below which pixels are not drawn");
    GUIContent dirLabel = new GUIContent(dir.displayName, "Number of viewing angles with distinct sprites");
    GUIContent InvLabel = new GUIContent(invRot.displayName, "Flip sprite rotation");
    GUIContent frameLabel = new GUIContent(frame.displayName, "Frame of the animation displayed when the framerate is 0. Use this to manually animate the sprite");
    GUIContent dimensionsLabel = new GUIContent("Sprite Sheet Dimensions", "Number of columns and rows in the sprite sheet");
    GUIContent totalLabel = new GUIContent("Number of animation frames", "Total number of sprites per direction, or columns multiplied by rows minus any empty frames for sprite sheets");
    GUIContent frameRateLabel = new GUIContent("Framerate", "Frames per second to flip through the sprites. Set to 0 and use the manual frame number to manually animate the sprite");

    bool sheetKey = target.IsKeywordEnabled("SPRITE_SHEET");
    EditorGUI.BeginChangeCheck();
    GUILayout.Label("");
    int textureType = EditorGUILayout.IntPopup("Texture Array Format", sheetKey ? 1 : 0, textureTypes, textureTypeVal);
    if (EditorGUI.EndChangeCheck())
    {
      switch(textureType)
      {
        case 0:
          target.DisableKeyword("SPRITE_SHEET");
          break;
        case 1:
          target.EnableKeyword("SPRITE_SHEET");
          break;
        default:
          target.DisableKeyword("SPRITE_SHEET");
          break;
      }
    }
    EditorGUILayout.Space();
      editor.TexturePropertySingleLine(mainTexLabel, mainTex, color);
    EditorGUILayout.Space();
    editor.ShaderProperty(light, lightLabel);
    if (alphaClip != null)
    {
      editor.ShaderProperty(alphaClip, alphaClipLabel);
    }
    editor.ShaderProperty(dir, dirLabel);
    editor.ShaderProperty(invRot, InvLabel);
    editor.ShaderProperty(frame, frameLabel);


    EditorGUI.BeginChangeCheck();
    GUILayout.Label("");
    float framerate = EditorGUI.FloatField(GUILayoutUtility.GetLastRect(), frameRateLabel, params1.vectorValue.w);
    Vector4 dim = new Vector4();
    float total = 0;
    GUILayout.Label("");
    total = EditorGUI.FloatField(GUILayoutUtility.GetLastRect(), totalLabel, params1.vectorValue.z);
    if (sheetKey && params1 != null)
    {
      GUILayout.Label("");
      dim = EditorGUI.Vector2Field(GUILayoutUtility.GetLastRect(), dimensionsLabel, params1.vectorValue);
    }
    if (EditorGUI.EndChangeCheck())
    {
      if (sheetKey && params1 != null)
      {
        params1.vectorValue = new Vector4(dim.x, dim.y, total, framerate);
      }
      else
      {
        params1.vectorValue = new Vector4(params1.vectorValue.x, params1.vectorValue.y, total, framerate);
      }
    }
    EditorGUILayout.Space();
    EditorGUILayout.Space();
    editor.RenderQueueField();

  }
}
