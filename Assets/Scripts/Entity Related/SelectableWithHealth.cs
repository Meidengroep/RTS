using UnityEngine;
using System.Collections;

/// <summary>
/// Contains functionality and methods used by objects that can be selected, ordered around and that have health.
/// </summary>
public abstract class SelectableWithHealth : MonoBehaviour
{
    public float MaxHealth;
    public float HealthRegeneration;
    public Transform HealthbarPosition;
    public BuildingsAndUnitsEnum Type;
    public float ViewRange;
    public GameObject Model;
    public Player ControllingPlayer;

    public float AttackDamage;
    public float AttackSpeed; // Attacks per second
    public float AttackRange;
    protected float attackTimer;

    protected float currentHealth;
    protected bool dead;

    abstract public void RightClick(SoftwareMouse mouse, RaycastHit raycastHit);
    abstract public SelectionMarker GetSelectionMarker();
    abstract protected void OnDeath();

    public void Start()
    {
        attackTimer = 1 / AttackSpeed;
    }

    public bool IsVisible()
    {
        if (ControllingPlayer.gameObject.activeSelf || (ControllingPlayer.EnemyPlayer.gameObject.activeSelf && !ControllingPlayer.EnemyPlayer.FogOfWar.IsInFog(this.gameObject.transform.position)))
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Initialize the health components to proper values so the object does not start dead.
    /// </summary>
    protected void InitHealth()
    {
        currentHealth = MaxHealth;
        dead = false;
    }

    /// <summary>
    /// Gets the current health of this object.
    /// </summary>
    /// <returns>The current health of this object.</returns>
    public float GetHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Deals damage to the health of this object. Also handles dying.
    /// </summary>
    /// <param name="damage">The damage that this object should take.</param>
    public void TakeDamage(float damage)
    {
        if (dead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        if (currentHealth == 0)
        {
            dead = true;
            OnDeath();
        }
    }

    /// <summary>
    /// Heals the object by a given amount.
    /// </summary>
    /// <param name="healing">The amount to heal.</param>
    public void Heal(float healing)
    {
        currentHealth += healing;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
    }

    /// <summary>
    /// Check if the object is dead.
    /// </summary>
    /// <returns>Whether the object is dead or not.</returns>
    public bool IsDead()
    {
        return dead;
    }

    /// <summary>
    /// Do a step of natural health regeneration of this object.
    /// </summary>
    protected void UpdateRegen()
    {
        Heal(HealthRegeneration * Time.deltaTime);
    }

    public void Update()
    {
        if (IsVisible())
        {
            Model.SetActive(true);
        }
        else Model.SetActive(false);

        attackTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Checks if the unit can attack, and if so, reset any cooldowns/timers for the attack.
    /// </summary>
    /// <returns>Whether the unit can attack.</returns>
    public bool Attack(SelectableWithHealth other)
    {
        if (attackTimer <= 0)
        {
            attackTimer = 1 / AttackSpeed;
            other.TakeDamage(AttackDamage);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check whether this SelectableWithHealth can attack a given GameObject.
    /// </summary>
    /// <param name="other">The GameObject you try to attack.</param>
    /// <returns>If you can attack other.</returns>
    public bool CanAttackOther(GameObject other)
    {
        if (other == null)
            return false;

        if (PlayerControlHelper.IsPlayerControlled(other.layer))
        {
            if (!PlayerControlHelper.SamePlayerControl(this.gameObject.layer, other.layer) && (other.tag == Tags.Unit || other.tag == Tags.Building))
                return true;
        }

        return false;
    }
}
