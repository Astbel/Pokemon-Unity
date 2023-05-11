using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum FacingDirection
{
    Up, Down, Right, Left
}


public class CharacterAnim : MonoBehaviour
{
    //Sprite
    [SerializeField] List<Sprite> walkDownSprite;
    [SerializeField] List<Sprite> walkUpSprite;
    [SerializeField] List<Sprite> walkRightSprite;
    [SerializeField] List<Sprite> walkLeftSprite;
    [SerializeField] List<Sprite> SurftSprite;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    //Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    public bool IsSurfing { get; set; }
    bool wasPreviouslyMoving;
    //States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;

    //Ref
    SpriteRenderer spriteRenderer;

    /*Frames default 0.16f*/
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprite, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprite, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprite, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprite, spriteRenderer);
        SetFacingDirection(defaultDirection);
        // currentAnim = walkDownAnim;
    }

    /*判斷腳色是否移動,如果沒有設置default為腳色預設動作*/
    private void Update()
    {
        if (!IsSurfing)
        {
            var prevAnim = currentAnim;

            if (MoveX == 1)
                currentAnim = walkRightAnim;
            else if (MoveX == -1)
                currentAnim = walkLeftAnim;
            else if (MoveY == 1)
                currentAnim = walkUpAnim;
            else if (MoveY == -1)
                currentAnim = walkDownAnim;
            /*確認是否有移動如果有的畫則重制*/
            if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
                currentAnim.Start();


            if (IsMoving)
                currentAnim.HandleUpdate();
            else
                spriteRenderer.sprite = currentAnim.Frames[0];
        }
        else
        {
            if (MoveX == 1)//右邊
                spriteRenderer.sprite=SurftSprite[2];
            else if (MoveX == -1)//左邊
                spriteRenderer.sprite=SurftSprite[3];
            else if (MoveY == 1)//上
                spriteRenderer.sprite=SurftSprite[1];
            else if (MoveY == -1)//下
                spriteRenderer.sprite=SurftSprite[0];
        }
        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
    }

    public FacingDirection DefaultDirection { get => defaultDirection; }

}
