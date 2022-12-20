using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject target;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip swirlClip;
    [SerializeField] private AudioClip basicVictorySound;
    [SerializeField] private AudioClip[] victoryClip;
    [SerializeField] private float suctionSpeed;
    private static int lastSound;
    private bool playerHasWon;
    private Vector3 velocity;
    private static Guid _playerSucceedsGuid;
    public bool finishedEntrance;

    private void Start()
    {
        EventSystem.Current.RegisterListener<WinningEvent>(OnPlayerSucceedsLevel, ref _playerSucceedsGuid);
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(OnPlayerDeathEvent, ref _playerSucceedsGuid);
    }

    private void OnPlayerSucceedsLevel(WinningEvent winningEvent)
    {
        if (!playerHasWon)
        {
            GameController.SetInputLockState(true);
            _audioSource.PlayOneShot(swirlClip);
            target = winningEvent.TargetGameObject;
            playerHasWon = true;
            _animator.SetBool("Won", true);
        }
    }

    public void FinishEntrance()
    {
        Debug.Log("yes, entrance is now true");
        finishedEntrance = true;
    }

    private AudioClip GetVictorySound()
    {
        Random rnd = new Random();
        int i;
        do
        {
            i = rnd.Next(victoryClip.Length);
        } while (i == lastSound);

        lastSound = i;
        return victoryClip[i];
    }

    public void FinishAnimation()
    {
        _audioSource.PlayOneShot(basicVictorySound);
        StartCoroutine(PlaySecondPartOfVictorySound());
        FindObjectOfType<IngameMenu>().Pause(1);
    }

    private IEnumerator PlaySecondPartOfVictorySound()
    {
        yield return new WaitWhile(()=> _audioSource.isPlaying);
        _audioSource.PlayOneShot(GetVictorySound());
    }

    public void OnPlayerDeathEvent(PlayerDeathEvent playerDeathEvent)
    {
        playerDeathEvent.SourceGameObject.GetComponent<Animator>().SetBool("dead", true);
        playerDeathEvent.SourceGameObject.transform.Find("VFX_Fire").gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (playerHasWon && target != null)
        {
            MoveToTarget();
            transform.position += velocity * Time.fixedDeltaTime;
        }
    }

    public void MoveToTarget()
    {
        Vector3 nextPos = Vector3.MoveTowards(transform.position, target.transform.position,
            suctionSpeed * Time.fixedDeltaTime);
        if (nextPos == transform.position)
        {
            velocity = Vector3.zero;
        }
        else
        {
            float speedX = (Math.Abs(nextPos.x) - Math.Abs(transform.position.x)) * 50;
            float speedY = (Math.Abs(nextPos.y) - Math.Abs(transform.position.y)) * 50;

            velocity.x = speedX;
            velocity.y = speedY;
        }
    }

    public void StartLevelAnimation()
    {
        _audioSource.PlayOneShot(swirlClip);
    }

    public void StartLevel()
    {
        GameController.UnpauseGame();
        LevelStartEvent levelStartEvent = new LevelStartEvent()
        {
        };
        EventSystem.Current.FireEvent(levelStartEvent);
    }
}