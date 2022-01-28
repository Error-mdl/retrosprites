using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextureArrayCreator : EditorWindow {

    public TextureList TexList;
    TextureList oldTexList;
    public CubemapList CubeList;
    CubemapList oldCubeList;
    public string[] TexType = new string[] { "Texture 2D", "Cubemap"};
    public int type = 0;

    public string[] filters = new string[] {"Point (No Filtering)", "Bilinear", "Trilinear"};
    public FilterMode filterMode = FilterMode.Bilinear;

    public bool copyMips = true;
    public float mipMapBias = 0;

    public string[] wraps = new string[] { "Repeat", "Clamp", "Mirror", "Mirror Once", "Per Axis" };
    public int wrapMode = 0;
    public string[] wrapsAxis = new string[] { "Repeat", "Clamp", "Mirror", "Mirror Once" };
    public TextureWrapMode wrapModeU = TextureWrapMode.Repeat;
    public TextureWrapMode wrapModeV = TextureWrapMode.Repeat;
    
    public int anisoLevel = 1;

    public struct TextureSettings
    {
        public FilterMode filterMode;
        public bool copyMips;
        public float mipMapBias;
        public TextureWrapMode wrapModeU;
        public TextureWrapMode wrapModeV;
        public int anisoLevel;
    }


    public bool showProperties = false;

    [MenuItem("Tools/Create Texture Array")]
    public static void ShowWindow()
    {
        GetWindow<TextureArrayCreator>(true, "Create Texture Array", true);
    }

    void OnGUI()
    {
        GUILayout.Label("All images must have the exact same dimensions, format, and number of mip levels!", EditorStyles.boldLabel);
        //Cubemap creator is non-functional
        //type = EditorGUILayout.Popup(type, TexType);
        //if (type == 0)
        //{
            GUI2DArray();
        //}
        //else
        //{
        //    GUICubeArray();
        //}
    }



    private void GUI2DArray()
    {
        oldTexList = TexList;
        GUILayout.Label("Texture List");
        TexList = EditorGUILayout.ObjectField("", TexList, typeof(TextureList), false) as TextureList;
       
        /* Copy the settings over from the first texture when a new texture list is put in the object field **/
        if (oldTexList != TexList && TexList != null && TexList.TexArray.Length > 0 && TexList.TexArray[0] != null)
        {
            filterMode = TexList.TexArray[0].filterMode;
            anisoLevel = TexList.TexArray[0].anisoLevel;
            wrapModeU = TexList.TexArray[0].wrapModeU;
            wrapModeV = TexList.TexArray[0].wrapModeV;
            if (wrapModeU == wrapModeV)
            {
                wrapMode = (int)wrapModeU;
            }
            else
            {
                wrapMode = 4;
            }
        }

        /* Do we want mip maps? if so, copy them from the source textures **/
        copyMips = EditorGUILayout.Toggle("Copy Mip Maps", copyMips);
        if (copyMips)
        {
            mipMapBias = EditorGUILayout.FloatField("    Mip Map Bias", mipMapBias);
        }

        filterMode = (FilterMode)EditorGUILayout.Popup("Filter Mode", (int)filterMode, filters);

        /* Set the wrap mode to the same value on both axes unless wrapMode is 4 (Per Axis), then show separate options**/
        wrapMode = EditorGUILayout.Popup("Wrap Mode", wrapMode, wraps);
        if (wrapMode == 4)
        {
            wrapModeU = (TextureWrapMode)EditorGUILayout.Popup("    U Axis", (int)wrapModeU, wrapsAxis);
            wrapModeV = (TextureWrapMode)EditorGUILayout.Popup("    V Axis", (int)wrapModeV, wrapsAxis);
        }
        else
        {
            wrapModeU = (TextureWrapMode)wrapMode;
            wrapModeV = (TextureWrapMode)wrapMode;
        }

        anisoLevel = EditorGUILayout.IntSlider("Aniso Level", anisoLevel, 0, 16);

        /* Show the user the settings that are going to be copied over from the source textures directly **/
        showProperties = EditorGUILayout.Foldout(showProperties, "Inherited Properties", true);
        if (showProperties && TexList != null)
        {
            printProperties(TexList.TexArray);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Array", GUILayout.Width(100), GUILayout.Height(30)))
        {
            TextureSettings settings = new TextureSettings();
            settings.filterMode = filterMode;
            settings.copyMips = copyMips;
            settings.mipMapBias = mipMapBias;
            settings.wrapModeU = wrapModeU;
            settings.wrapModeV = wrapModeV;
            settings.anisoLevel = anisoLevel;
            CopyListIntoArray(TexList, settings);
        }
    }


    private void GUICubeArray()
    {
        oldCubeList = CubeList;
        GUILayout.Label("Cubemap List");
        CubeList = EditorGUILayout.ObjectField("", CubeList, typeof(CubemapList), false) as CubemapList;
        
        /* Copy the settings over from the first texture when a new texture list is put in the object field **/
        if (oldCubeList != CubeList && CubeList != null && CubeList.TexArray.Length > 0 && CubeList.TexArray[0] != null)
        {
            filterMode = CubeList.TexArray[0].filterMode;
            anisoLevel = CubeList.TexArray[0].anisoLevel;
            wrapModeU = CubeList.TexArray[0].wrapModeU;
            wrapModeV = CubeList.TexArray[0].wrapModeV;
            if (wrapModeU == wrapModeV)
            {
                wrapMode = (int)wrapModeU;
            }
            else
            {
                wrapMode = 4;
            }
        }

        /* Do we want mip maps? if so, copy them from the source textures **/
        copyMips = EditorGUILayout.Toggle("Copy Mip Maps", copyMips);
        if (copyMips)
        {
            mipMapBias = EditorGUILayout.FloatField("    Mip Map Bias", mipMapBias);
        }

        filterMode = (FilterMode)EditorGUILayout.Popup("Filter Mode", (int)filterMode, filters);

        /* Set the wrap mode to the same value on both axes unless wrapMode is 4 (Per Axis), then show separate options**/
        wrapMode = EditorGUILayout.Popup("Wrap Mode", wrapMode, wraps);
        if (wrapMode == 4)
        {
            wrapModeU = (TextureWrapMode)EditorGUILayout.Popup("    U Axis", (int)wrapModeU, wrapsAxis);
            wrapModeV = (TextureWrapMode)EditorGUILayout.Popup("    V Axis", (int)wrapModeV, wrapsAxis);
        }
        else
        {
            wrapModeU = (TextureWrapMode)wrapMode;
            wrapModeV = (TextureWrapMode)wrapMode;
        }

        anisoLevel = EditorGUILayout.IntSlider("Aniso Level", anisoLevel, 0, 16);

        /* Show the user the settings that are going to be copied over from the source textures directly **/
        showProperties = EditorGUILayout.Foldout(showProperties, "Inherited Properties", true);
        if (showProperties && CubeList != null)
        {
            printPropertiesCUBE(CubeList.TexArray);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Array", GUILayout.Width(100), GUILayout.Height(30)))
        {
            TextureSettings settings = new TextureSettings();
            settings.filterMode = filterMode;
            settings.copyMips = copyMips;
            settings.mipMapBias = mipMapBias;
            settings.wrapModeU = wrapModeU;
            settings.wrapModeV = wrapModeV;
            settings.anisoLevel = anisoLevel;
            CopyListIntoArrayCUBE(CubeList, settings);
        }
    }

    /*
     * Check to see if a given texture from the nth position in the array has the same dimensions, format,
     * and number of mip levels as the first element
     **/
    private bool HasSameSettings(Texture2D first, Texture2D nth, int index)
    {

        int fail = 0;
        if (first.width == nth.width && first.height == nth.height)
        {
            if (first.format == nth.format)
            {
                if (first.mipmapCount != nth.mipmapCount)
                    fail = 3;
            }
            else fail = 2;
        }
        else fail = 1;
        switch (fail)
        {
            case 1:
                EditorUtility.DisplayDialog("Textures not the same dimensions",
                    string.Format("Texture {0} has size of {1}x{2}, expected {3}x{4}",
                              index, nth.width, nth.height, first.width, first.height),
                    "ok");
                return false;
            case 2:
                EditorUtility.DisplayDialog("Textures not the format",
                   string.Format("Texture {0} has {1} format, expected {2}",
                             index, nth.format.ToString(), first.format.ToString()),
                   "ok");
                return false;
            case 3:
                EditorUtility.DisplayDialog("Not all Textures have the same number of mip levels",
                   string.Format("Texture {0} has {1} mip levels, expected {2}",
                             index, nth.mipmapCount, first.mipmapCount),
                   "ok");
                return false;
            default:
                return true;
        }
    }


  /*
   * Check to see if a given Cubemap from the nth position in the array has the same dimensions, format,
   * and number of mip levels as the first element
   **/
    private bool HasSameSettingsCUBE(Cubemap first, Cubemap nth, int index)
    {

        int fail = 0;
        if (first.width == nth.width && first.height == nth.height)
        {
            if (first.format == nth.format)
            {
                if (first.mipmapCount != nth.mipmapCount)
                    fail = 3;
            }
            else fail = 2;
        }
        else fail = 1;
        switch (fail)
        {
            case 1:
                EditorUtility.DisplayDialog("Textures not the same dimensions",
                    string.Format("Texture {0} has size of {1}x{2}, expected {3}x{4}",
                              index, nth.width, nth.height, first.width, first.height),
                    "ok");
                return false;
            case 2:
                EditorUtility.DisplayDialog("Textures not the format",
                   string.Format("Texture {0} has {1} format, expected {2}",
                             index, nth.format.ToString(), first.format.ToString()),
                   "ok");
                return false;
            case 3:
                EditorUtility.DisplayDialog("Not all Textures have the same number of mip levels",
                   string.Format("Texture {0} has {1} mip levels, expected {2}",
                             index, nth.mipmapCount, first.mipmapCount),
                   "ok");
                return false;
            default:
                return true;
        }
    }

    /* Print out the width, height, texture format, and number of mip levels **/
    private void printProperties(Texture2D[] TexArray)
    {
        int width, height, depth;
        string format;
        int mipCount;

        if (TexArray.Length > 0 && TexArray[0] != null)
        {
            width = TexArray[0].width;
            height = TexArray[0].height;
            depth = TexArray.Length;
            format = TexArray[0].format.ToString();
            mipCount = TexArray[0].mipmapCount;
        }
        else
        {
            width = 0;
            height = 0;
            depth = 0;
            format = "none";
            mipCount = 0;
        }

        GUILayout.Label("These settings are copied from the first image in the list");
        EditorGUILayout.LabelField("Texture Format:", format);
        EditorGUILayout.LabelField("Texture Dimensions:", string.Format("{0}x{1}", width, height));
        EditorGUILayout.LabelField("Mip Levels:", mipCount.ToString());
        EditorGUILayout.LabelField("Number of Images:", depth.ToString());
    }

    /* Print out the width, height, texture format, and number of mip levels **/
    private void printPropertiesCUBE(Cubemap[] TexArray)
    {
        int width, height, depth;
        string format;
        int mipCount;

        if (TexArray.Length > 0 && TexArray[0] != null)
        {
            width = TexArray[0].width;
            height = TexArray[0].height;
            depth = TexArray.Length;
            format = TexArray[0].format.ToString();
            mipCount = TexArray[0].mipmapCount;
        }
        else
        {
            width = 0;
            height = 0;
            depth = 0;
            format = "none";
            mipCount = 0;
        }

        GUILayout.Label("These settings are copied from the first image in the list");
        EditorGUILayout.LabelField("Texture Format:", format);
        EditorGUILayout.LabelField("Texture Dimensions:", string.Format("{0}x{1}", width, height));
        EditorGUILayout.LabelField("Mip Levels:", mipCount.ToString());
        EditorGUILayout.LabelField("Number of Images:", depth.ToString());
    }

    /* Given a list of textures and settings, copy over each texture into a texture2darray and save it as a file **/
    private void CopyListIntoArray(TextureList List, TextureSettings Settings)
    {
        if (List != null && List.TexArray.Length > 0)
        {
            if (List.TexArray[0] == null)
            {
                EditorUtility.DisplayDialog("First element unassigned", "Element 0 of the texture list is empty!", "ok");
                return;
            }
            Texture2DArray output = new Texture2DArray(List.TexArray[0].width, List.TexArray[0].height, List.TexArray.Length, List.TexArray[0].format, Settings.copyMips && List.TexArray[0].mipmapCount > 1);
            output.mipMapBias = Settings.mipMapBias;
            output.filterMode = Settings.filterMode;
            output.wrapModeU = Settings.wrapModeU;
            output.wrapModeV = Settings.wrapModeV;
            output.anisoLevel = Settings.anisoLevel;

            bool consistentSettings = true;
            for (int i = 0; i < List.TexArray.Length; i++)
            {
                /* Stop if one of the elements in the list is empty **/
                if (List.TexArray[i] == null)
                {
                    EditorUtility.DisplayDialog("Element unassigned", string.Format("Element {0} of the texture list is empty!", i), "ok");
                    return;
                }

                /* Stop if the texture being copied doesn't have the same settings as the first element of the array **/
                consistentSettings = HasSameSettings(List.TexArray[0], List.TexArray[i], i);
                if (consistentSettings == false)
                    return;

                /* Copy the contents of the texture into the corresponding slice of the Texture2DArray, and copy over all the mips if copyMips is true **/
                if (Settings.copyMips)
                { 
                    for (int j = 0; j < List.TexArray[0].mipmapCount; j++)
                    {
                        Graphics.CopyTexture(List.TexArray[i], 0, j, output, i, j);
                    }
                }
                else
                {
                    Graphics.CopyTexture(List.TexArray[i], 0, 0, output, i, 0);
                }
            }

            output.Apply(false);

            string path = EditorUtility.SaveFilePanelInProject("Save Array", List.name + "_texarray.asset", "asset",
                "Please enter a file name to save the texture array to");
            if (path.Length != 0)
            {
                AssetDatabase.CreateAsset(output, path);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("No texture list selected", "You must enter a non-empty texture list", "ok");
        }
    }


    /* Given a list of textures and settings, copy over each texture into a texture2darray and save it as a file **/
    private void CopyListIntoArrayCUBE(CubemapList List, TextureSettings Settings)
    {
        if (List != null && List.TexArray.Length > 0)
        {
            if (List.TexArray[0] == null)
            {
                EditorUtility.DisplayDialog("First element unassigned", "Element 0 of the texture list is empty!", "ok");
                return;
            }
            CubemapArray output = new CubemapArray(List.TexArray[0].width, List.TexArray.Length, List.TexArray[0].format, Settings.copyMips);
            output.mipMapBias = Settings.mipMapBias;
            output.filterMode = Settings.filterMode;
            output.wrapModeU = Settings.wrapModeU;
            output.wrapModeV = Settings.wrapModeV;
            output.anisoLevel = Settings.anisoLevel;

            bool consistentSettings = true;
            for (int i = 0; i < List.TexArray.Length; i++)
            {
                /* Stop if one of the elements in the list is empty **/
                if (List.TexArray[i] == null)
                {
                    EditorUtility.DisplayDialog("Element unassigned", string.Format("Element {0} of the texture list is empty!", i), "ok");
                    return;
                }

                /* Stop if the texture being copied doesn't have the same settings as the first element of the array **/
                consistentSettings = HasSameSettingsCUBE(List.TexArray[0], List.TexArray[i], i);
                if (consistentSettings == false)
                    return;

                /* Copy the contents of the texture into the corresponding slice of the Texture2DArray, and copy over all the mips if copyMips is true **/
                if (Settings.copyMips)
                {
                    for (int j = 0; j < List.TexArray[0].mipmapCount; j++)
                    {
                        Graphics.CopyTexture(List.TexArray[i], 0, j, output, i, j);
                    }
                }
                else
                {
                    Graphics.CopyTexture(List.TexArray[i], 0, 0, output, i, 0);
                }
            }

            output.Apply(false);

            string path = EditorUtility.SaveFilePanelInProject("Save Array", List.name + "_texarray.asset", "asset",
                "Please enter a file name to save the texture array to");
            if (path.Length != 0)
            {
                AssetDatabase.CreateAsset(output, path);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("No texture list selected", "You must enter a non-empty texture list", "ok");
        }
    }
}
