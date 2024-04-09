using PlethoraV2.Utility;
using PlethoraV2.Values;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransformTweening : MonoBehaviour
{
    public enum StartMode
    {
        OnStart,
        OnEnable,
        Other
    }
    public enum TransparencyAnimationSource
    {
        None,
        SpriteRender,
        Image,
        CanvasGroup,
        Text
    }
    [System.Serializable]
    public struct AnimationSettings
    {
        public Vector3 position; 
        //public Quaternion rotation;
        public Vector3 rotation;
        public Vector3 scale;
        public float transparency;

        public AnimationSettings(GameObject target)
        {
            this.position = target.transform.localPosition;
            this.rotation = target.transform.localEulerAngles;
            this.scale = target.transform.localScale;

            var sprite = target.GetComponent<SpriteRenderer>();

            transparency = 1;

            if (sprite)
            {
                transparency = sprite.color.a;
            }
            else
            {
                var canvasGroup = target.GetComponent<CanvasGroup>();

                if (canvasGroup)
                {
                    transparency = canvasGroup.alpha;
                }
                else
                {
                    var image = target.GetComponent<Image>();

                    if (image)
                    {
                        transparency = image.color.a;
                    }
                    else
                    {
                        var text = target.GetComponent<TextMeshProUGUI>();

                        if (text)
                        {
                            transparency = text.color.a;
                        }
                    }
                }
            }
        }
    }

    [SerializeField] bool editorMode;
    [SerializeField,Range(0,1)] float editorAnimationProgress;
    [SerializeField] GameObject animatedObject;

    [SerializeField, Space(20)] private bool recordInitialTransformSnapshot;
    [SerializeField] public AnimationSettings initialTransform;
    [SerializeField] private bool recordFinalTransformSnapshot;
    [SerializeField] public AnimationSettings finalTransform;

    [SerializeField, Space(20)] bool ignoreTimeScale;
    [SerializeField] bool inverseInitialization;
    [SerializeField] StartMode startMode = StartMode.Other;
    [SerializeField] float fowardDelay = 0;
    [SerializeField] float backwardDelay = 0;
    [SerializeField] float animationDuration = 1;
    [SerializeField] ToggableValue<AnimationCurve> positionCurve;
    [SerializeField] ToggableValue<AnimationCurve> rotationCurve;
    [SerializeField] ToggableValue<AnimationCurve> scaleCurve;
    [SerializeField] ToggableValue<AnimationCurve> transparencyCurve;

    [SerializeField, Space(20)] UnityEvent onFowardCompleted;
    [SerializeField] UnityEvent onBackwardCompleted;
    [SerializeField] UnityEvent onCompleted;

    [SerializeField, ShowOnly] bool playingAnimation;

    [SerializeField, ShowOnly] public TransparencyAnimationSource transparencyAnimationSource;

    private SpriteRenderer spriteRenderer;
    private CanvasGroup canvasGroup;
    private Image image;
    private TextMeshProUGUI text;

    bool isInit;

    Coroutine coroutine;

    private void Start()
    {
        if (inverseInitialization)
        {
            ResetState(finalTransform, true);
        }
        else
        {
            ResetState(initialTransform, true);
        }
        if (startMode == StartMode.OnStart) PlayFoward();

    }

    private void OnEnable()
    {
        Initialize();

        if (inverseInitialization)
        {
            ResetState(finalTransform, true);
        }
        else
        {
            ResetState(initialTransform, true);
        }

        if (startMode == StartMode.OnEnable)
        {
            PlayFoward();
        }
    }

    private void Initialize()
    {
        if (isInit) return;
        isInit = true;

        editorMode = false;

        GetDependencies();
    }

    private void GetDependencies()
    {
        spriteRenderer = animatedObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer)
        {
            transparencyAnimationSource = TransparencyAnimationSource.SpriteRender;
        }
        else
        {
            canvasGroup = animatedObject.GetComponent<CanvasGroup>();

            if (canvasGroup)
            {
                transparencyAnimationSource = TransparencyAnimationSource.CanvasGroup;
            }
            else
            {
                image = animatedObject.GetComponent<Image>();

                if (image)
                {
                    transparencyAnimationSource = TransparencyAnimationSource.Image;

                }
                else
                {
                    text = animatedObject.GetComponent<TextMeshProUGUI>();
                    if (text)
                    {
                        transparencyAnimationSource = TransparencyAnimationSource.Text;

                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        playingAnimation = false;
    }

    private void OnValidate()
    {
        if (animatedObject == null) return;

        if (recordInitialTransformSnapshot)
        {
            recordInitialTransformSnapshot = false;
            initialTransform = new(animatedObject);
        }

        if (recordFinalTransformSnapshot)
        {
            recordFinalTransformSnapshot = false;
            finalTransform = new(animatedObject);
        }

        if (editorMode)
        {
            AnimationSettings a;
            AnimationSettings b;

            if (inverseInitialization == false)
            {
                a = initialTransform;
                b = finalTransform;
            }
            else
            {
                a = finalTransform;
                b = initialTransform;
            }
            GetDependencies();
            LerpAnimation(a, b, editorAnimationProgress);
        }
    }


    public void PlayFoward()
    {
        if (playingAnimation) return;
        playingAnimation = true;

        ResetState(initialTransform);
        coroutine = StartCoroutine(DoAnimation());
    }
    public void PlayBackward()
    {
        if (playingAnimation) return;
        playingAnimation = true;

        ResetState(finalTransform);
        coroutine = StartCoroutine(DoAnimation(true));
    }

    IEnumerator DoAnimation(bool invert = false)
    {
        AnimationSettings a;
        AnimationSettings b;

        if (invert == false)
        {
            if(fowardDelay > 0) yield return new WaitForSeconds(fowardDelay);

            a = initialTransform;
            b = finalTransform;
        }
        else
        {
            if (backwardDelay > 0) yield return new WaitForSeconds(backwardDelay);

            a = finalTransform;
            b = initialTransform;
        }
            
        float currentDuration = animationDuration;

        while (currentDuration >= 0)
        {
            float progress = currentDuration / animationDuration;
            currentDuration -= ignoreTimeScale == false? Time.deltaTime : Time.unscaledDeltaTime;

            if (progress < 0) progress = 0;

            progress = 1 - progress;

            LerpAnimation(a, b, progress);

            yield return null;
        }

        playingAnimation = false;

        if(invert == false)
        {
            onFowardCompleted.Invoke();
        }
        else
        {
            onBackwardCompleted.Invoke();
        }
        onCompleted.Invoke();
    }

    private void LerpAnimation(AnimationSettings a, AnimationSettings b, float progress)
    {
        var lerpPos = a.position != b.position || a.position != animatedObject.transform.localPosition || animatedObject.transform.localPosition != b.position;
        
        if (lerpPos && positionCurve.Enabled)
        {
            animatedObject.transform.localPosition = Vector3.Lerp(a.position, b.position, positionCurve.Value.Evaluate(progress));
        }

        var lerpRot = a.rotation != b.rotation || a.rotation != animatedObject.transform.localEulerAngles || animatedObject.transform.localEulerAngles != b.rotation;

        if (lerpRot && rotationCurve.Enabled)
        {
            animatedObject.transform.localEulerAngles = Vector3.Slerp(a.rotation, b.rotation, rotationCurve.Value.Evaluate(progress));
        }

        var lerpScale = a.scale != b.scale || a.scale != animatedObject.transform.localScale || animatedObject.transform.localScale != b.scale;


        if (lerpScale && scaleCurve.Enabled)
        {
            animatedObject.transform.localScale = Vector3.Lerp(a.scale, b.scale, scaleCurve.Value.Evaluate(progress));
        }

        if (a.transparency != b.transparency && transparencyCurve.Enabled)
        {
            if (transparencyAnimationSource == TransparencyAnimationSource.SpriteRender)
            {
                var color = spriteRenderer.color;
                color.a = MathUtil.LerpUnclamped(a.transparency, b.transparency, transparencyCurve.Value.Evaluate(progress));
                spriteRenderer.color = color;
            }
            else if (transparencyAnimationSource == TransparencyAnimationSource.CanvasGroup)
            {
                canvasGroup.alpha = MathUtil.LerpUnclamped(a.transparency, b.transparency, transparencyCurve.Value.Evaluate(progress));
            }
            else if (transparencyAnimationSource == TransparencyAnimationSource.Image)
            {
                var color = image.color;
                color.a = MathUtil.LerpUnclamped(a.transparency, b.transparency, transparencyCurve.Value.Evaluate(progress));
                image.color = color;
            }
            else if (transparencyAnimationSource == TransparencyAnimationSource.Text)
            {
                var color = text.color;
                color.a = MathUtil.LerpUnclamped(a.transparency, b.transparency, transparencyCurve.Value.Evaluate(progress));
                text.color = color;
            }
        }
    }

    public void ResetState(AnimationSettings animationSettings, bool forceTransformReset = false)
    {
        if (positionCurve.Enabled || forceTransformReset) animatedObject.transform.localPosition = animationSettings.position;
        if (rotationCurve.Enabled || forceTransformReset) animatedObject.transform.localEulerAngles = animationSettings.rotation;
        if (scaleCurve.Enabled || forceTransformReset) animatedObject.transform.localScale = animationSettings.scale;


        if (transparencyCurve.Enabled)
        {
            if (transparencyAnimationSource == TransparencyAnimationSource.SpriteRender)
            {
                var color = spriteRenderer.color;
                color.a = animationSettings.transparency;
                spriteRenderer.color = color;
            }
            else if (transparencyAnimationSource == TransparencyAnimationSource.CanvasGroup)
            {
                canvasGroup.alpha = animationSettings.transparency;
            }
            else if (transparencyAnimationSource == TransparencyAnimationSource.Image)
            {
                var color = image.color;
                color.a = animationSettings.transparency;
                image.color = color;
            }
            else if (transparencyAnimationSource == TransparencyAnimationSource.Text)
            {
                var color = text.color;
                color.a = animationSettings.transparency;
                text.color = color;
            }
        }
    }
}
