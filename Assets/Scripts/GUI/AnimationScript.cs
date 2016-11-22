using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Model;

public class AnimationScript : MonoBehaviour {
    
    private Image _img;

    private EMPLOYEE _employee;

    private byte _frame;
    private byte _sprite;
    
    private string _imgPath
    {
        get
        {
            return "Sprites/Employees/" + _employee.employee_name + '/' + _expression.ToString() + "/frame " + _frame.ToString() + '/' + _action.ToString()  + '_' + _sprite.ToString();
        }
    }

    public enum Expression
    {
        angry,
        blink,
        bowing,
        cry,
        dam,
        despair,
        glitter,
        love,
        pain,
        smile,
        troubled,
        wink
    }
    private Expression _expression;

    public enum Action
    {
        alert,
        jump,
        prone,
        shootF,
        sit,
        stand1,
        walk1
    }
    private Action _action;

    private void OnEnable()
    {
        _expression = Expression.dam;
        _action = Action.shootF;

        StartCoroutine(AnimateAction());
        StartCoroutine(AnimateExpression());
    }

    public IEnumerator Animate()
    {
        Texture2D texture2d;

        while(true)
        {
            texture2d = (Texture2D)Resources.Load(_imgPath);
            if(texture2d == null)
            {
                texture2d = null;
            }
            else
            {
                _img.sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
            }
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }



    public IEnumerator AnimateAction()
    {
        while (true)
        { 
            switch (_action)
            {
                case Action.alert:
                    _sprite = 0;
                    yield return new WaitForSeconds(.4f);
                    _sprite = 1;
                    yield return new WaitForSeconds(.4f);
                    _sprite = 2;
                    yield return new WaitForSeconds(.3f);
                    _sprite = 3;
                    yield return new WaitForSeconds(.2f);
                    _sprite = 4;
                    yield return new WaitForSeconds(.2f);
                    _sprite = 3;
                    yield return new WaitForSeconds(.3f);
                    _sprite = 2;
                    yield return new WaitForSeconds(.4f);
                    _sprite = 1;
                    yield return new WaitForSeconds(.4f);
                    break;
                case Action.jump:
                    _sprite = 0;
                    yield return new WaitForSeconds(3f);
                    break;
                case Action.prone:
                    _sprite = 0;
                    yield return new WaitForSeconds(3f);
                    break;
                case Action.shootF:
                    _sprite = 0;
                    yield return new WaitForSeconds(.5f);
                    _sprite = 1;
                    yield return new WaitForSeconds(.5f);
                    _img.transform.localScale = new Vector3(-_img.transform.localScale.x, 1, 1);
                    break;
                case Action.sit:
                    _sprite = 0;
                    yield return new WaitForSeconds(3f);
                    break;
                case Action.stand1:
                    _sprite = 0;
                    yield return new WaitForSeconds(.4f);
                    _sprite = 1;                     
                    yield return new WaitForSeconds(.4f);
                    _sprite = 2;                     
                    yield return new WaitForSeconds(.4f);
                    _sprite = 3;                     
                    yield return new WaitForSeconds(.4f);
                    _sprite = 4;                     
                    yield return new WaitForSeconds(.4f);
                    _sprite = 3;                     
                    yield return new WaitForSeconds(.4f);
                    _sprite = 2;                     
                    yield return new WaitForSeconds(.4f);
                    _sprite = 1;                     
                    yield return new WaitForSeconds(.4f);
                    break;
                case Action.walk1:
                    _sprite = 0;
                    yield return new WaitForSeconds(.2f);
                    _sprite = 1;             
                    yield return new WaitForSeconds(.2f);
                    _sprite = 2;             
                    yield return new WaitForSeconds(.2f);
                    _sprite = 3;             
                    yield return new WaitForSeconds(.2f);
                    _sprite = 2;             
                    yield return new WaitForSeconds(.2f);
                    _sprite = 1;             
                    yield return new WaitForSeconds(.2f);
                    break;
            }
        }

        yield break;
    }



    public IEnumerator AnimateExpression()
    {
        while (true)
        {
            switch (_expression)
            {
                case Expression.angry:
                    _frame = 0;
                    yield return new WaitForEndOfFrame();
                    break;
                case Expression.blink:
                    _frame = 0;
                    yield return new WaitForSeconds(Random.Range(3, 5));
                    _frame = 1;
                    yield return new WaitForEndOfFrame();
                    _frame = 2;
                    yield return new WaitForEndOfFrame();
                    _frame = 2;
                    yield return new WaitForEndOfFrame();
                    break;
                case Expression.dam:
                    _frame = 0;
                    yield return new WaitForSeconds(0.3f);
                    _frame = 1;
                    yield return new WaitForSeconds(0.3f);
                    break;
                case Expression.despair:
                    _frame = 0;
                    yield return new WaitForSeconds(0.3f);
                    _frame = 1;
                    yield return new WaitForSeconds(0.3f);
                    break;
                case Expression.glitter:
                    _frame = 0;
                    yield return new WaitForSeconds(0.3f);
                    _frame = 1;
                    yield return new WaitForSeconds(0.3f);
                    break;
                case Expression.troubled:
                    _frame = 0;
                    yield return new WaitForSeconds(3f);
                    break;
            }
        }

        yield break;
    }
    

    public void SetupAnimation(Image targetImg, EMPLOYEE employeeToAnimate)
    {
        if(targetImg == null || employeeToAnimate == null)
        {
            Debug.LogError("::CRITICAL:: targetImg OR employeeToAnimate came null");
            return;
        }

        _img = targetImg;

        //If this is the first time the employee in foucs is assigned
        if(_employee == null)
        {
            _employee = employeeToAnimate;
            StartCoroutine(Animate());
        }

        _employee = employeeToAnimate;
    }
    
    

    public void SetExpression(Expression expression)
    {
        _sprite = 0;
        _frame = 0;
        _expression = expression;
    }

    public void SetAction(Action action)
    {
        _sprite = 0;
        _frame = 0;
        _action = action;

        _img.transform.localScale = new Vector3(1, 1, 1);
    }
    
}
