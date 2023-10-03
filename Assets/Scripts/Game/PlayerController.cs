using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IResettable
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float speed, turnSensitivity;
    [SerializeField] private GameObject ForceCollider;
    [SerializeField] private GameObject extraColliders;

    private int _currentDiamondProfit;
    private Hole _currentHole;
    private float _turnSpeed;
    
    public float SpeedModifier;

    #region PlayerEvents
    public event Action OnPlay;
    public event Action OnFinish;
    public event Action OnLaunched;
    public event Action OnTurnInput;
    public event Action OnFill;
    public event Action OnWin;
    public event Action OnFailed;
    #endregion
    
    void Awake()
    {
        FindObjectOfType<GameManager>().OnReset += ResetToDefault;
    }

    void FixedUpdate()
    {
        OnPlay?.Invoke();
    }

    public void VelocityControl() => playerBody.velocity = new Vector3( Mathf.Clamp(_turnSpeed, -10, 10), playerBody.velocity.y, speed + SpeedModifier);

    private void MovementInput()
    {
        if (Input.GetMouseButton(0))
        {
            if(_turnSpeed >= 1) OnTurnInput?.Invoke();
            _turnSpeed = Input.GetAxis("Mouse X") * turnSensitivity;
        }
        else _turnSpeed = 0;
    }

    private IEnumerator WaitHoleToPass(Hole hole, float wait)
    {
        playerBody.isKinematic = true;
        ForceCollider.SetActive(true);

        yield return new WaitForSeconds(wait);

        ForceCollider.SetActive(false);
        if (hole.isFilled)
        {
            OnFill?.Invoke();
            playerBody.isKinematic = false;
        }
        else
        {
            OnFailed?.Invoke();
            Destroy(hole);
        }
    }

    private IEnumerator WaitPlayerToStop()
    {
        yield return new WaitForSeconds(4);
        GameManager.DiamondCount += _currentDiamondProfit;
        playerBody.isKinematic = true;
        OnWin?.Invoke();
    }
    
    public void ResetToDefault()
    {
        OnPlay += MovementInput;
        
        playerBody = GetComponent<Rigidbody>();
        playerBody.isKinematic = false;
        SetBodyConstraints(RigidbodyConstraints.FreezeRotation);
        extraColliders.SetActive(true);
        SpeedModifier = 0;
    }
  
    public void SetBodyConstraints(RigidbodyConstraints constraints) => playerBody.constraints = constraints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unload Point"))
        {
            _currentHole = other.GetComponentInParent<Hole>();
            other.enabled = false;
            
            StartCoroutine(WaitHoleToPass(_currentHole, 5));
        }
        
        if (other.CompareTag("Ball"))
        {
            if (other.TryGetComponent(out Rigidbody body))
            {
                body.AddForce(Vector3.forward * 10, ForceMode.VelocityChange);
            }
        }
        
        if (other.CompareTag("Finish"))
        {
            OnFinish?.Invoke();
            OnPlay -= MovementInput;

            other.enabled = false;
            SetBodyConstraints(RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ);
            extraColliders.SetActive(false);
            
            _turnSpeed = 0;
        }
        
        if (other.gameObject.CompareTag("Launch"))
        {
            OnLaunched?.Invoke();
            OnPlay -= VelocityControl;
            playerBody.AddForce(Vector3.down * 3, ForceMode.VelocityChange);
        }

        if (other.CompareTag("Landing Tile"))
        {
            _currentDiamondProfit = int.Parse(other.GetComponentInChildren<Text>().text);
            StopCoroutine(nameof(WaitPlayerToStop));
            StartCoroutine(nameof(WaitPlayerToStop));
        }
    }
}
