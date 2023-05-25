using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace LightGive
{
	[CustomEditor(typeof(TransitionManager))]
	public class TransitionManagerEditor : Editor
	{
		private Vector3 m_centerPosition;
		private Material m_previewMat;
		private float m_lerp = 0.5f;

		private SerializedProperty m_defaultFlashDurationProp;
		private SerializedProperty m_defaultFlashWhiteDurationProp;
		private SerializedProperty m_defaultFlashColorProp;

		private SerializedProperty m_defaultEffectTypeProp;
		private SerializedProperty m_defaultTransDurationProp;
		private SerializedProperty m_defaultEffectColorProp;
		private SerializedProperty m_ruleTexProp;
		private SerializedProperty m_transShaderProp;
		private SerializedProperty m_animCurveProp;
		private SerializedProperty m_isInvertProp;

		private bool m_isCustom
		{
			get
			{
				if (m_defaultEffectTypeProp == null)
					return false;
				return m_defaultEffectTypeProp.enumValueIndex == (int)TransitionManager.EffectType.Custom;
			}
		}

		private void OnEnable()
		{
			//SerializedProperty取得
			m_defaultFlashDurationProp = serializedObject.FindProperty("m_defaultFlashDuration");
			m_defaultFlashWhiteDurationProp = serializedObject.FindProperty("m_defaultFlashWhiteDuration");
			m_defaultFlashColorProp = serializedObject.FindProperty("m_defaultFlashColor");

			m_defaultEffectTypeProp = serializedObject.FindProperty("m_defaultEffectType");
			m_defaultTransDurationProp = serializedObject.FindProperty("m_defaultTransDuration");
			m_defaultEffectColorProp = serializedObject.FindProperty("m_defaultEffectColor");
			m_ruleTexProp = serializedObject.FindProperty("m_ruleTex");
			m_transShaderProp = serializedObject.FindProperty("m_transShader");
			m_animCurveProp = serializedObject.FindProperty("m_animCurve");
			m_isInvertProp = serializedObject.FindProperty("m_isInvert");

			var transShader = Shader.Find(TransitionManager.TransitionShaderName);
			serializedObject.Update();
			m_transShaderProp.objectReferenceValue = transShader;

			m_previewMat = new Material((Shader)transShader);
			if ((Texture)m_ruleTexProp.objectReferenceValue != null)
			{
				SetMaterialParamAll();
			}
			serializedObject.ApplyModifiedProperties();
		}


		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("闪光默认参数", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(m_defaultFlashDurationProp, new GUIContent("持续时间"));
			EditorGUILayout.PropertyField(m_defaultFlashWhiteDurationProp, new GUIContent("闪光持续时间"));
			EditorGUILayout.PropertyField(m_defaultFlashColorProp, new GUIContent("闪光颜色"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("转场默认参数", EditorStyles.boldLabel);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_defaultEffectTypeProp, new GUIContent("转场类型"));
			if (EditorGUI.EndChangeCheck())
			{
				SetMaterialParamAll();
			}

			EditorGUILayout.PropertyField(m_defaultTransDurationProp, new GUIContent("持续时间"));
			EditorGUILayout.PropertyField(m_animCurveProp, new GUIContent("动画曲线"));

			//ChangeColor
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_defaultEffectColorProp, new GUIContent("贴图颜色"));
			if (EditorGUI.EndChangeCheck())
			{
				m_previewMat.SetColor(TransitionManager.ShaderParamColor, m_defaultEffectColorProp.colorValue);
			}

			//当转场类型为自定义时
			if (m_isCustom)
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("自定义设置", EditorStyles.boldLabel);

				if (m_transShaderProp.objectReferenceValue == null)
				{
					EditorGUILayout.HelpBox("请将“LightGive/unlist/TransitionShader”着色器添加到项目中", MessageType.Error);
				}
				else
				{
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.PropertyField(m_transShaderProp);
					EditorGUI.EndDisabledGroup();

                    //检查规则图像的更改
                    EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(m_ruleTexProp);
					if (EditorGUI.EndChangeCheck())
					{
						SetMaterialParamAll();
					}

                    //检查翻转的角度
                    EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(m_isInvertProp);
					if (EditorGUI.EndChangeCheck())
					{
						m_previewMat.SetFloat(TransitionManager.ShaderParamFloatInvert, m_isInvertProp.boolValue ? 1.0f : 0.0f);
					}

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("所有参数恢复到默认");

                    //根据规则图像是否设定来区分处理
                    if ((Texture)m_ruleTexProp.objectReferenceValue != null)
					{
						//ルール画像が設定されている時
						//スライダーの変更をチェック
						EditorGUI.BeginChangeCheck();
						m_lerp = EditorGUILayout.Slider(m_lerp, 0.0f, 1.0f);
						var val = Mathf.Clamp01(m_animCurveProp.animationCurveValue.Evaluate(m_lerp));

						if (EditorGUI.EndChangeCheck())
						{
							m_previewMat.SetFloat(TransitionManager.ShaderParamFloatCutoff, val);
						}

						float contextWidth = (float)typeof(EditorGUIUtility).GetProperty("contextWidth", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
						var w = contextWidth - 30f;

						var sizes = UnityStats.screenRes.Split('x');


						var h = w * (float.Parse(sizes[1]) / float.Parse(sizes[0]));
						GUILayout.Box(GUIContent.none, GUILayout.Width(w), GUILayout.Height(h));
						var lastRect = GUILayoutUtility.GetLastRect();
						lastRect.width -= 4;
						lastRect.height -= 4;
						lastRect.x += 2;
						lastRect.y += 2;
						EditorGUI.DrawPreviewTexture(lastRect, Texture2D.whiteTexture, m_previewMat);
					}
					else
					{
						EditorGUILayout.HelpBox("请设置规则纹理", MessageType.Info);
					}
				}
			}
			serializedObject.ApplyModifiedProperties();
		}

		void SetMaterialParamAll()
		{
			m_previewMat.SetTexture(TransitionManager.ShaderParamTextureGradation, (Texture)m_ruleTexProp.objectReferenceValue);
			m_previewMat.SetColor(TransitionManager.ShaderParamColor, m_defaultEffectColorProp.colorValue);
			m_previewMat.SetFloat(TransitionManager.ShaderParamFloatInvert, m_isInvertProp.boolValue ? 1.0f : 0.0f);
			m_previewMat.SetFloat(TransitionManager.ShaderParamFloatCutoff, m_lerp);

		}
	}
}