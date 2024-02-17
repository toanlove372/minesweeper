using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridUtils
{
    [System.Serializable]
    public class MapData
    {
        public int width;
        public int height;
        [SerializeField]
        private int[] rawData;

        public int[] RawData { get => rawData; private set => rawData = value; }

        public MapData(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.InitGridData();
        }

        public void InitGridData()
        {
            int totalCell = this.width * this.height;
            this.rawData = new int[totalCell];

            //SetGridBorder(totalCell);
        }

        public int GetValue(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
            {
                return -1;
            }

            return this.rawData[y * this.width + x];
        }

        public void SetValue(int x, int y, int value)
        {
            this.rawData[y * this.width + x] = value;
        }

        public void SetValues(byte[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                this.rawData[i] = values[i];
            }
        }

        private void SetGridBorder(int totalCell)
        {
            for (int i = 0; i < this.width; i++)
            {
                this.rawData[i] = 1;
                this.rawData[totalCell - 1 - i] = 1;
            }

            for (int i = 1; i < this.height; i++)
            {
                this.rawData[i * this.width] = 1;
                this.rawData[i * this.width - 1] = 1;
            }
        }
    }

    public static class MapDataUtils
    {
        private const int PixelPerCell = 12;

        static MapDataUtils()
        {
            colorMapArray = new Color32[colorMap.Length][];
            for (int i = 0; i < colorMapArray.Length; i++)
            {
                Color32[] arrayColor = new Color32[PixelPerCell * PixelPerCell];
                for (int j = 0; j < arrayColor.Length; j++)
                {
                    arrayColor[j] = colorMap[i];
                }
                colorMapArray[i] = arrayColor;
            }
        }

        private static Color32[] colorMap =
        {
            Color.white,
            Color.grey,
            Color.blue,
            Color.red,
            Color.cyan,
            Color.magenta,
        };

        private static Color32[][] colorMapArray;

        public static Texture GenerateThumbnail(this MapData mapData)
        {
            Texture2D texture = new Texture2D(96, 96, TextureFormat.RGBA32, false);
            int width = mapData.width;
            int height = mapData.height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int value = mapData.GetValue(j, i);
                    texture.SetPixels32(j * PixelPerCell, i * PixelPerCell, PixelPerCell, PixelPerCell, colorMapArray[value], 0);
                }
            }
            texture.Apply();

            return texture;
        }
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "Map", menuName = "Grid Tile Map", order = 1)]
    public class MapDataObject : ScriptableObject
    {
        public MapData data;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapDataObject))]
    public class MapDataEditor : Editor
    {
        SerializedProperty widthProp;
        SerializedProperty heightProp;

        private void OnEnable()
        {
            this.widthProp = serializedObject.FindProperty("data.width");
            this.heightProp = serializedObject.FindProperty("data.height");
        }

        public override void OnInspectorGUI()
        {
            MapData selected = ((MapDataObject)target).data;

            EditorGUILayout.PropertyField(this.widthProp);
            EditorGUILayout.PropertyField(this.heightProp);

            if (GUILayout.Button("Init"))
            {
                selected.InitGridData();
            }

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);
            }

            if (selected.RawData == null || selected.RawData.Length < (selected.width * selected.height))
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            for (int i = selected.height - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < selected.width; j++)
                {
                    int value = selected.RawData[i * selected.width + j];
                    switch (value)
                    {
                        case 0:
                            GUI.backgroundColor = Color.white;
                            break;
                        case 1:
                            GUI.backgroundColor = Color.gray;
                            break;
                        case 2:
                            GUI.backgroundColor = Color.blue;
                            break;
                        case 3:
                            GUI.backgroundColor = Color.red;
                            break;
                        case 4:
                            GUI.backgroundColor = Color.cyan;
                            break;
                        case 5:
                            GUI.backgroundColor = Color.magenta;
                            break;
                    }

                    selected.RawData[i * selected.width + j] = EditorGUILayout.IntField(selected.RawData[i * selected.width + j], GUILayout.Width(25));


                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.BeginDisabledGroup(false);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
#endif

}