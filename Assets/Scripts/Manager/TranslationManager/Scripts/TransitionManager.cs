﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 使用uGUI的场景转换管理器.
/// 在场景加载时，可以使用图像填充方法和规则图像生成.
/// 另外，你还可以制作像flash这样的转场
/// </summary>
public class TransitionManager : Singleton<TransitionManager>
{
	public const string TransitionShaderName = "LightGive/Unlit/TransitionShader";
	public const string ShaderParamTextureGradation = "_Gradation";
	public const string ShaderParamFloatInvert = "_Invert";
	public const string ShaderParamFloatCutoff = "_Cutoff";
	public const string ShaderParamColor = "_Color";

	//Flash
	[SerializeField]
	private float m_defaultFlashDuration = 0.1f;
	[SerializeField]
	private float m_defaultFlashWhiteDuration = 0.05f;
	[SerializeField]
	private Color m_defaultFlashColor = Color.white;

	//Transition
	public EffectType m_defaultEffectType = EffectType.Fade;

    public float m_defaultTransDuration = .5f;
	[SerializeField]
	private Texture m_ruleTex;
	[SerializeField]
	private Color m_defaultEffectColor = Color.black;
	[SerializeField]
	private AnimationCurve m_animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	[SerializeField]
	private Shader m_transShader;
	[SerializeField]
	private bool m_isInvert = false;

	private int m_texCount = 0;
	private bool m_isTransition = false;
	private bool m_isFlash = false;
	private Sprite m_transitionSprite;
	private Image m_transImage;
	private Image m_transImageFlash;
	private RawImage m_transRawImage;
	private CanvasScaler m_baseCanvasScaler;
	private Canvas m_baseCanvas;

	#region Prop
	public Color defaultFlashColor { get { return m_defaultFlashColor; } }
	public float defaultFlashDuration { get { return m_defaultFlashDuration; } }
	public float defaultFlashWhiteDuration { get { return m_defaultFlashWhiteDuration; } }

	public Color defaultEffectColor { get { return m_defaultEffectColor; } }
	public EffectType defaultEffectType { get { return m_defaultEffectType; } }
	public float defaultTransDuration { get { return m_defaultTransDuration; } }
    #endregion

    /// <summary>
    /// 效果类型
    /// Custom是一种使用规则图像的转换方法
    /// </summary>
    public enum EffectType
	{
		Fade = 0,

		Horizontal_Right = 1,
		Horizontal_Left = 2,

		Vertical_Top = 3,
		Vertical_Bottom = 4,

		Radial90_TopRight = 5,
		Radial90_TopLeft = 6,
		Radial90_BottomRight = 7,
		Radial90_BottomLeft = 8,

		Radial180_Right = 9,
		Radial180_Left = 10,
		Radial180_Top = 11,
		Radial180_Bottom = 12,

		Radial360_Right = 13,
		Radial360_Left = 14,
		Radial360_Top = 15,
		Radial360_Bottom = 16,

		Custom = 17,
	}

	protected void Awake()
	{
		Init();
	}

	void Init()
	{
		CreateCanvas();
		CreateRawImage();
		m_transImage = CreateImage();
		m_transImageFlash = CreateImage();

		Texture2D plainTex = CreateTexture2D();
		m_transitionSprite = Sprite.Create(plainTex, new Rect(0, 0, 32, 32), Vector2.zero);


		m_transitionSprite.name = "TransitionSpeite";
		m_transImage.sprite = m_transitionSprite;
		m_transImage.type = Image.Type.Filled;
		m_transImage.fillAmount = 1.0f;

		m_transImageFlash.gameObject.name = "FlashImage";
		m_transImageFlash.sprite = m_transitionSprite;
		m_transImageFlash.type = Image.Type.Simple;

	}

	void CreateCanvas()
	{
		if (this.gameObject.GetComponent<Canvas>() != null)
			m_baseCanvas = this.gameObject.GetComponent<Canvas>();
		else
			m_baseCanvas = this.gameObject.AddComponent<Canvas>();

		m_baseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		m_baseCanvas.sortingOrder = 999;
		m_baseCanvas.pixelPerfect = false;

		if (this.gameObject.GetComponent<CanvasScaler>() != null)
			m_baseCanvasScaler = this.gameObject.GetComponent<CanvasScaler>();
		else
			m_baseCanvasScaler = this.gameObject.AddComponent<CanvasScaler>();

		m_baseCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		m_baseCanvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
		m_baseCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
		m_baseCanvasScaler.referencePixelsPerUnit = 100.0f;

	}

	Image CreateImage()
	{
		GameObject transImageObj;
		transImageObj = new GameObject("Transition Image");
		transImageObj.transform.SetParent(this.gameObject.transform);
		transImageObj.transform.position = Vector3.zero;
		transImageObj.transform.localPosition = Vector3.zero;
		transImageObj.transform.localScale = Vector3.one;
		transImageObj.transform.rotation = Quaternion.identity;
		var i = transImageObj.AddComponent<Image>();
		i.color = m_defaultEffectColor;
		i.sprite = null;
		RectTransform transImageRectTransfrm;
		transImageRectTransfrm = transImageObj.GetComponent<RectTransform>();
		transImageRectTransfrm.anchorMin = Vector2.zero;
		transImageRectTransfrm.anchorMax = Vector2.one;
		transImageRectTransfrm.pivot = new Vector2(0.5f, 0.5f);
		transImageRectTransfrm.localPosition = Vector3.zero;
		transImageRectTransfrm.sizeDelta = Vector3.zero;
		transImageObj.SetActive(false);
		return i;
	}

	void CreateRawImage()
	{
		GameObject transRawImageObj;
		transRawImageObj = new GameObject("Transition Raw Image");
		transRawImageObj.transform.SetParent(this.gameObject.transform);
		transRawImageObj.transform.position = Vector3.zero;
		transRawImageObj.transform.localPosition = Vector3.zero;
		transRawImageObj.transform.localScale = Vector3.one;
		transRawImageObj.transform.rotation = Quaternion.identity;
		m_transRawImage = transRawImageObj.AddComponent<RawImage>();
		m_transRawImage.color = m_defaultEffectColor;
		m_transRawImage.texture = null;
		RectTransform transRawImageRectTransfrm;
		transRawImageRectTransfrm = transRawImageObj.GetComponent<RectTransform>();
		transRawImageRectTransfrm.anchorMin = Vector2.zero;
		transRawImageRectTransfrm.anchorMax = Vector2.one;
		transRawImageRectTransfrm.pivot = new Vector2(0.5f, 0.5f);
		transRawImageRectTransfrm.localPosition = Vector3.zero;
		transRawImageRectTransfrm.sizeDelta = Vector3.zero;
		m_transRawImage.gameObject.SetActive(false);
	}

	Texture2D CreateTexture2D()
	{
		var tex = new Texture2D(32, 32, TextureFormat.RGB24, false);
		for (int i = 0; i < tex.width; i++)
		{
			for (int ii = 0; ii < tex.height; ii++)
			{
				tex.SetPixel(i, ii, Color.white);
			}
		}
		return tex;
	}

	private IEnumerator TransitionAction(UnityAction _act, float _transtime, EffectType _effectType, Color _effectColor)
	{
		if (m_isTransition)
			yield break;

		m_isTransition = true;
		SceneTransitionInit(_effectColor, _effectType);

		float t = Time.time;
		float lp = 0.0f;

		while (Time.time - t < _transtime)
		{
			lp = m_animCurve.Evaluate((Time.time - t) / _transtime);
			SceneTransitionDirection(lp, _effectType);
			yield return null;
		}

		_act.Invoke();

		t = Time.time;
		lp = 0.0f;

		m_animCurve = FlipCurve(m_animCurve);

		while (Time.time - t < _transtime)
		{
			lp = m_animCurve.Evaluate((Time.time - t) / _transtime);
			SceneTransitionDirection(lp, _effectType);
			yield return null;
		}
		m_animCurve = FlipCurve(m_animCurve);

		if (_effectType == EffectType.Custom)
		{
			SceneTransitionDirection(0.0f, _effectType);
			m_transRawImage.gameObject.SetActive(false);
		}
		else
		{
			m_transImage.fillAmount = 0.0f;
			m_transImage.gameObject.SetActive(false);
		}

		m_isTransition = false;
	}

	private IEnumerator _Flash(int _flashCount, float _fadeDuration, float _whiteDuration, Color _flashColor)
	{
		if (m_isFlash)
			yield break;

		int loopCount = 0;
		m_isFlash = true;
		m_transImageFlash.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		m_transImageFlash.gameObject.SetActive(true);

		while (loopCount < _flashCount)
		{
			float t = Time.time;
			float lp = 0.0f;
			while (Time.time - t < _fadeDuration)
			{
				lp = (Time.time - t) / _fadeDuration;
				m_transImageFlash.color = Color.Lerp(new Color(_flashColor.r, _flashColor.g, _flashColor.b, 0.0f), _flashColor, lp); yield return null;
			}

			m_transImageFlash.color = _flashColor;
			yield return new WaitForSeconds(_whiteDuration);

			t = Time.time;
			lp = 0.0f;
			while (Time.time - t < _fadeDuration)
			{
				lp = 1.0f - ((Time.time - t) / _fadeDuration);
				m_transImageFlash.color = Color.Lerp(new Color(_flashColor.r, _flashColor.g, _flashColor.b, 0.0f), _flashColor, lp);
				yield return null;
			}

			m_transImageFlash.color = Color.clear;
			yield return new WaitForEndOfFrame();
			loopCount++;
		}

		m_transImageFlash.gameObject.SetActive(false);
		m_isFlash = false;
	}

	private AnimationCurve FlipCurve(AnimationCurve _curve)
	{
		AnimationCurve newCurve = new AnimationCurve();
		for (int i = 0; i < _curve.length; i++)
		{
			Keyframe key = _curve[i];
			key.time = 1f - key.time;
			key.inTangent = key.inTangent * -1f;
			key.outTangent = key.outTangent * -1f;
			newCurve.AddKey(key);
		}
		return newCurve;
	}


	void SceneTransitionDirection(float _lerp, EffectType _effectType)
	{
		switch (_effectType)
		{
			case EffectType.Fade:
				m_transImage.color = new Color(m_transImage.color.r, m_transImage.color.g, m_transImage.color.b, _lerp);
				break;

			case EffectType.Custom:
				m_transRawImage.material.SetFloat(ShaderParamFloatCutoff, _lerp);
				break;

			default:
				m_transImage.fillAmount = _lerp;
				break;
		}
	}

	void SceneTransitionInit(Color _effectColor, EffectType _effectType)
	{
		switch (_effectType)
		{
			case EffectType.Fade:
				m_transImage.fillAmount = 1.0f;
				break;

			case EffectType.Custom:
				Material mat = new Material(m_transShader);
				m_transRawImage.material = mat;
				m_transRawImage.material.SetTexture(ShaderParamTextureGradation, m_ruleTex);
				m_transRawImage.material.SetFloat(ShaderParamFloatInvert, m_isInvert ? 1.0f : 0.0f);
				break;

			case EffectType.Horizontal_Right:
				m_transImage.fillMethod = Image.FillMethod.Horizontal;
				m_transImage.fillOrigin = (int)Image.OriginHorizontal.Right;
				break;
			case EffectType.Horizontal_Left:
				m_transImage.fillMethod = Image.FillMethod.Horizontal;
				m_transImage.fillOrigin = (int)Image.OriginHorizontal.Left;
				break;
			case EffectType.Vertical_Top:
				m_transImage.fillMethod = Image.FillMethod.Vertical;
				m_transImage.fillOrigin = (int)Image.OriginVertical.Top;
				break;
			case EffectType.Vertical_Bottom:
				m_transImage.fillMethod = Image.FillMethod.Vertical;
				m_transImage.fillOrigin = (int)Image.OriginVertical.Bottom;
				break;
			case EffectType.Radial90_TopRight:
				m_transImage.fillMethod = Image.FillMethod.Radial90;
				m_transImage.fillOrigin = (int)Image.Origin90.TopRight;
				break;
			case EffectType.Radial90_TopLeft:
				m_transImage.fillMethod = Image.FillMethod.Radial90;
				m_transImage.fillOrigin = (int)Image.Origin90.TopLeft;
				break;
			case EffectType.Radial90_BottomRight:
				m_transImage.fillMethod = Image.FillMethod.Radial90;
				m_transImage.fillOrigin = (int)Image.Origin90.BottomRight;
				break;
			case EffectType.Radial90_BottomLeft:
				m_transImage.fillMethod = Image.FillMethod.Radial90;
				m_transImage.fillOrigin = (int)Image.Origin90.BottomLeft;
				break;
			case EffectType.Radial180_Right:
				m_transImage.fillMethod = Image.FillMethod.Radial180;
				m_transImage.fillOrigin = (int)Image.Origin180.Right;
				break;
			case EffectType.Radial180_Left:
				m_transImage.fillMethod = Image.FillMethod.Radial180;
				m_transImage.fillOrigin = (int)Image.Origin180.Left;
				break;
			case EffectType.Radial180_Top:
				m_transImage.fillMethod = Image.FillMethod.Radial180;
				m_transImage.fillOrigin = (int)Image.Origin180.Top;
				break;
			case EffectType.Radial180_Bottom:
				m_transImage.fillMethod = Image.FillMethod.Radial180;
				m_transImage.fillOrigin = (int)Image.Origin180.Bottom;
				break;
			case EffectType.Radial360_Right:
				m_transImage.fillMethod = Image.FillMethod.Radial360;
				m_transImage.fillOrigin = (int)Image.Origin360.Right;
				break;
			case EffectType.Radial360_Left:
				m_transImage.fillMethod = Image.FillMethod.Radial360;
				m_transImage.fillOrigin = (int)Image.Origin360.Left;
				break;
			case EffectType.Radial360_Top:
				m_transImage.fillMethod = Image.FillMethod.Radial360;
				m_transImage.fillOrigin = (int)Image.Origin360.Top;
				break;
			case EffectType.Radial360_Bottom:
				m_transImage.fillMethod = Image.FillMethod.Radial360;
				m_transImage.fillOrigin = (int)Image.Origin360.Bottom;
				break;
		}

		switch (_effectType)
		{
			case EffectType.Fade:
				m_transImage.fillAmount = 1.0f;
				m_transImage.color = new Color(_effectColor.r, _effectColor.g, _effectColor.b, 0.0f);
				break;
			case EffectType.Custom:
				m_transRawImage.color = _effectColor;
				break;
			default:
				m_transImage.fillAmount = 0.0f;
				m_transImage.color = _effectColor;
				break;
		}

		if (_effectType == EffectType.Custom)
		{
			m_transRawImage.gameObject.SetActive(true);
		}
		else
		{
			m_transImage.gameObject.SetActive(true);
		}

	}

	void SceneTransitionEnd(EffectType _effectType)
	{
		switch (_effectType)
		{
			case EffectType.Fade:
				m_transImage.fillAmount = 1.0f;
				break;
			default:
				m_transImage.fillAmount = 0.0f;
				break;
		}
	}

    #region Flash

    /// <summary>
    /// 闪烁屏幕.
    /// (所有默认参数)
    /// </summary>
    public void Flash()
	{
		StartCoroutine(_Flash(1, m_defaultFlashDuration, m_defaultFlashWhiteDuration, m_defaultFlashColor));
	}

    /// <summary>
    /// 闪烁屏幕.
    /// </summary>
    /// <param name="_flashCount">Count of flashes.</param>
    public void Flash(int _flashCount)
	{
		StartCoroutine(_Flash(_flashCount, m_defaultFlashDuration, m_defaultFlashWhiteDuration, m_defaultFlashColor));
	}

    /// <summary>
    /// 闪烁屏幕.
    /// </summary>
    /// <param name="_fadeDuration">Fade duration.</param>
    /// <param name="_whiteDuration">White duration.</param>
    public void Flash(float _fadeDuration, float _whiteDuration)
	{
		StartCoroutine(_Flash(1, _fadeDuration, _whiteDuration, m_defaultFlashColor));
	}

    /// <summary>
    /// 闪烁屏幕.
    /// </summary>
    /// <param name="_flashColor">Flash color.</param>
    public void Flash(Color _flashColor)
	{
		StartCoroutine(_Flash(1, m_defaultFlashDuration, m_defaultFlashWhiteDuration, _flashColor));
	}

    /// <summary>
    /// 闪烁屏幕.
    /// </summary>
    /// <param name="_flashCount">Count of flashes.</param>
    /// <param name="_fadeDuration">Fade duration.</param>
    /// <param name="_whiteDuration">White duration.</param>
    public void Flash(int _flashCount, float _fadeDuration, float _whiteDuration)
	{
		StartCoroutine(_Flash(_flashCount, _fadeDuration, _whiteDuration, m_defaultFlashColor));
	}

    /// <summary>
    /// 闪烁屏幕.
    /// </summary>
    /// <param name="_flashCount">Count of flashes.</param>
    /// <param name="_flashColor">Flash color.</param>
    public void Flash(int _flashCount, Color _flashColor)
	{
		StartCoroutine(_Flash(_flashCount, m_defaultFlashDuration, m_defaultFlashWhiteDuration, _flashColor));
	}

    /// <summary>
    /// 闪烁屏幕.
    /// </summary>
    /// <param name="_flashCount">Count of flashes.</param>
    /// <param name="_fadeDuration">Fade duration.</param>
    /// <param name="_whiteDuration">White duration.</param>
    /// <param name="_flashColor">Flash color.</param>
    public void Flash(int _flashCount, float _fadeDuration, float _whiteDuration, Color _flashColor)
	{
		StartCoroutine(_Flash(_flashCount, _fadeDuration, _whiteDuration, _flashColor));
	}

    #endregion

    #region LoadScene

    /// <summary>
    /// 加载带有过渡的场景.
    /// (All default parameter)
    /// </summary>
    /// <param name="_sceneName">Name of scene to load.</param>
    public void LoadScene(string _sceneName)
	{
		StartCoroutine(TransitionAction(
			() => SceneManager.LoadScene(_sceneName),
			m_defaultTransDuration,
			m_defaultEffectType,
			defaultEffectColor));
	}

    /// <summary>
    /// 加载带有过渡的场景.
    /// (All default parameter)
    /// </summary>
    /// <param name="_sceneName">Name of scene to load.</param>
    public void LoadScene(int index)
    {
        StartCoroutine(TransitionAction(
            () => SceneManager.LoadScene(index),
            m_defaultTransDuration,
            m_defaultEffectType,
            defaultEffectColor));
    }

    /// <summary>
    /// 加载带有过渡的场景.
    /// </summary>
    /// <param name="_sceneName">Name of scene to load.</param>
    /// <param name="_duration">Transition duration.</param>
    public void LoadScene(string _sceneName, float _duration)
	{
		StartCoroutine(TransitionAction(() => SceneManager.LoadScene(_sceneName), _duration, m_defaultEffectType, defaultEffectColor));
	}

    /// <summary>
    /// 加载带有过渡的场景.
    /// </summary>
    /// <param name="_sceneName">Name of scene to load.</param>
    /// <param name="_duration">Transition duration.</param>
    /// <param name="_effectType">Effect type.</param>
    /// <param name="_effectColor">Effect color.</param>
    public void LoadScene(string _sceneName, float _duration, EffectType _effectType, Color _effectColor)
	{
		StartCoroutine(TransitionAction(
			() =>
			SceneManager.LoadScene(_sceneName),
			_duration,
			_effectType,
			_effectColor));
	}

	#endregion

	#region ReLoadScene

	/// <summary>
	/// 重新加载这个场景.
	/// (All default parameter)
	/// </summary>
	public void ReLoadScene()
	{
		StartCoroutine(TransitionAction(
			() =>
			SceneManager.LoadScene(SceneManager.GetActiveScene().name),
			m_defaultTransDuration,
			m_defaultEffectType,
			m_defaultEffectColor));
	}

    /// <summary>
    /// 重新加载这个场景.
    /// </summary>
    /// <param name="_duration">Transition duration.</param>
    public void ReLoadScene(float _duration)
	{
		StartCoroutine(TransitionAction(
			() =>
			SceneManager.LoadScene(SceneManager.GetActiveScene().name),
			_duration,
			m_defaultEffectType,
			m_defaultEffectColor));
	}

    /// <summary>
    /// 重新加载这个场景.
    /// </summary>
    /// <param name="_effectType">Effect type.</param>
    public void ReLoadScene(EffectType _effectType)
	{
		StartCoroutine(TransitionAction(
			() =>
			SceneManager.LoadScene(SceneManager.GetActiveScene().name),
			m_defaultTransDuration,
			_effectType,
			m_defaultEffectColor));
	}

    /// <summary>
    /// 重新加载这个场景.
    /// </summary>
    /// <param name="_effectColor">Effect color.</param>
    public void ReLoadScene(Color _effectColor)
	{
		StartCoroutine(TransitionAction(
			() =>
			SceneManager.LoadScene(SceneManager.GetActiveScene().name),
			m_defaultTransDuration,
			m_defaultEffectType,
			_effectColor));
	}

    /// <summary>
    /// 重新加载这个场景
    /// </summary>
    /// <param name="_duration">Transition duration.</param>
    /// <param name="_effectType">Effect type.</param>
    /// <param name="_effectColor">Effect color.</param>
    public void ReLoadScene(float _duration, EffectType _effectType, Color _effectColor)
	{
		StartCoroutine(TransitionAction(
			() =>
			SceneManager.LoadScene(SceneManager.GetActiveScene().name),
			_duration,
			_effectType,
			_effectColor));
	}

    #endregion

    /// <summary>
    /// 开始场景转换效应.
    /// </summary>
    /// <param name="_duration">Transition duration.</param>
    /// <param name="_effectType">Effect type.</param>
    /// <param name="_effectColor">Effect color.</param>
    /// <param name="_act">Method to execute when screen is covered</param>
    public void StartTransitonEffect(float _duration, EffectType _effectType, Color _effectColor, UnityAction _act)
	{
		StartCoroutine(TransitionAction(_act, _duration, _effectType, _effectColor));
	}
}