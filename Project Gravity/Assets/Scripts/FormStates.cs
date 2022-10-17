using UnityEngine;

public class FormStates : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private MeshRenderer _meshRenderer;
    private Form _currentForm;
    private GameObject _parentGameObject;
    private PlayerMovement _playerMovement;
    private Rigidbody _rigidbody;
    private PlayerStats _playerStats;
    private int _cooldownCounter;
    private float _coolDown;
    private Form[] _allForms;
    
    private void Start()
    {
        // Initializes and stores references to the different mesh attributes to be altered throughout play.
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = gameObject.GetComponent<MeshCollider>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _parentGameObject = gameObject.transform.parent.gameObject;
        _playerMovement = _parentGameObject.GetComponent<PlayerMovement>();
        _rigidbody = _parentGameObject.GetComponent<Rigidbody>();

        _playerStats = _parentGameObject.GetComponent<PlayerStats>();
        _allForms = _playerStats.GetAllForms();
        _cooldownCounter = (int) _playerStats.GetFormSwitchCooldown() * 60;
        _coolDown = _playerStats.GetFormSwitchCooldown();
        _currentForm = _allForms[0];
        ChangeForm(0);
    }

    void FixedUpdate()
    {
        if (_cooldownCounter <= _coolDown * 60)
        {
            _cooldownCounter += 1;
            return;
        }
    }

    public void TriggerFormChange()
    {
        if (_currentForm.formName.Equals(_allForms[0].formName))
        {
            ChangeForm(1);
        }
        else
        {
            ChangeForm(0);
        }

        _cooldownCounter = 0;
    }

    private void ChangeForm(int formIndex)
    {
        _currentForm = _allForms[formIndex];
        _meshFilter.sharedMesh = _currentForm.formMesh;
        _meshCollider.sharedMesh = _currentForm.formMesh;
        _meshRenderer.material = _currentForm.formMaterial;
        if (_currentForm.canMove)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _playerMovement.RotateToPlane();
        }
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }

    public Form GetCurrentForm()
    {
        return _currentForm;
    }
}