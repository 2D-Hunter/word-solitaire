#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace StarChestCreator
{
    [CustomEditor(typeof(Chest))]
    public class ChestEditor : Editor
    {
        private SerializedProperty chestNameProperty;
        private SerializedProperty chestSpritesProperty;
        private SerializedProperty chestRewardsProperty;

        private void OnEnable()
        {
            // Initialize serialized properties
            chestNameProperty = serializedObject.FindProperty("chestName");
            chestSpritesProperty = serializedObject.FindProperty("chestSprites");
            chestRewardsProperty = serializedObject.FindProperty("chestRewards");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(chestNameProperty, new GUIContent("Chest Name"));

            // Display the chestSprites array with the ability to expand/collapse
            EditorGUILayout.PropertyField(chestSpritesProperty, new GUIContent("Chest Sprites"), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Chest Rewards", EditorStyles.boldLabel);

            // Loop through the chestRewards list
            for (int i = 0; i < chestRewardsProperty.arraySize; i++)
            {
                SerializedProperty rewardProperty = chestRewardsProperty.GetArrayElementAtIndex(i);
                SerializedProperty rewardItemProperty = rewardProperty.FindPropertyRelative("reward");
                SerializedProperty amountProperty = rewardProperty.FindPropertyRelative("amount");

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Reward {i + 1}", EditorStyles.boldLabel);

                // Remove button
                if (GUILayout.Button("Remove"))
                {
                    chestRewardsProperty.DeleteArrayElementAtIndex(i);
                    break; // Exit the loop to avoid errors after deletion
                }

                EditorGUILayout.EndHorizontal();

                // Display reward fields
                EditorGUILayout.PropertyField(rewardItemProperty, new GUIContent("Reward"));
                EditorGUILayout.PropertyField(amountProperty, new GUIContent("Amount"));

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            // Add Reward button
            if (GUILayout.Button("Add Reward"))
            {
                chestRewardsProperty.InsertArrayElementAtIndex(chestRewardsProperty.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif