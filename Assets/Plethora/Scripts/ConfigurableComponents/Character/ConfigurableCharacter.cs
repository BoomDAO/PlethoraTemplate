using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class ConfigurableCharacter : MonoBehaviour
{
    [field: SerializeField] public Transform AssetHolder { get; private set; }
    [field: SerializeField, ShowOnly] private Animator animator;

    public Animator Animator
    {
        get
        {
            if (animator == null)
            {
                if (AssetHolder.TryGetComponent<Animator>(out animator) == false)
                {
                    Debug.LogError("New skin doesn't have animator");
                }
            }
            //if(animator.enabled == false) animator.enabled = true;

            return animator;
        }
        private set { animator = value; }
    }
    public UnityEvent<GameObject> OnSkinChange { get; private set; } = new();//{ get; private set; } = new();

    [field: SerializeField, ShowOnly] private string reviveVFX;
    [field: SerializeField, ShowOnly] private string deathVFX;

    bool initSkin;
    bool initWeaponLeft;
    bool initWeaponRight;


    public void SetupHandTool(GameObject tool, bool left)
    {
        if(AssetHolder.childCount > 0)
        {
            if (tool == null) return;

            if (left)
            {
                if (initWeaponLeft) return;
                initWeaponLeft = true;
            }
            else
            {
                if (initWeaponRight) return;

                initWeaponRight = true;
            }



            var weaponHolder = FindChildRecursive(AssetHolder, left);
            if (weaponHolder == null) return;

            foreach (Transform currentTool in weaponHolder)
                Destroy(currentTool.gameObject);


            tool.transform.SetParent(weaponHolder);
            tool.transform.localPosition = Vector3.zero;
            tool.transform.localRotation = default;
            tool.transform.localScale = Vector3.one;

        }
    }


    private Transform FindChildRecursive(Transform source, bool left)
    {

        string nameToLookFor = left ? "WeaponHolderLeft" : "WeaponHolderRight";
        if (source.gameObject.name == nameToLookFor)
        {
            return source;
        }

        foreach (Transform child in source)
        {
            Transform result = FindChildRecursive(child.transform, left);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public void SetupSkin(GameObject skin)
    {
        initSkin = true;

        foreach (Transform t in AssetHolder)
        {
            Destroy(t.gameObject);
            break;
        }


        skin.transform.SetParent(AssetHolder);
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localRotation = default;
        skin.transform.localScale = Vector3.one;

        if (skin.TryGetComponent<Animator>(out var skinAnimator))
        {
            Animator.runtimeAnimatorController = skinAnimator.runtimeAnimatorController;
            Animator.avatar = skinAnimator.avatar;
            Animator.Rebind();
            skinAnimator.enabled = false;
        }

        OnSkinChange.Invoke(skin);
    }

    public void SetupEffects(string reviveVFX, string deathVFX)
    {
        this.reviveVFX = reviveVFX;
        this.deathVFX = deathVFX;
    }

    public void DoReviveEffect()
    {
        DoReviveEffect_().Forget();
    }
    public void DoDeathEffect()
    {
        DoDeathEffect_().Forget();
    }

    private async UniTaskVoid DoReviveEffect_()
    {
        var effect = await Addressables.InstantiateAsync(reviveVFX);

        effect.transform.position = transform.position + Vector3.up * 1f;
    }
    private async UniTaskVoid DoDeathEffect_()
    {
        var effect = await Addressables.InstantiateAsync(deathVFX);

        effect.transform.position = transform.position + Vector3.up * 1f;
    }


}
