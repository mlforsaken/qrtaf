using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class combo_builder : MonoBehaviour {
    public float countdown_time = 5;
    public float delay = 5;
    public float round_time = 60;
    public float break_time = 30.0f;
    public Button button;
    public Text gui;
    public Text combo_name;
    float timer = 0;
    float round_timer = 0;
    public Image fade_in_layer;
    public float fade_in_time = 5;

    Animator button_animator;

    AudioClip bell;
    AudioClip clip;
    AudioClip[] cliplist;
    AudioSource aus;
    enum State
    {
        wait_to_start,
        countdown,
        bell,
        play,
        idle,
    }

    public static combo_builder inst;

    void Awake()
    {
        inst = this;

        button_animator = button.GetComponent<Animator>();
    }

    public void start_training()
    {
        button.gameObject.SetActive(false);
        gui.gameObject.SetActive(true);
//        fade_in_layer.color = new Color(0, 0, 0, 0);
        state = State.countdown;
    }
    State _state = State.countdown;

    State state { get { return _state; } set { _set_state(value); } }

    void set_gui_time(float t)
    {
        int min = (int)(t / 60.0f);
        int sec = (int)(t - 60 * min);

        gui.text = string.Format("{0:D2}:{1:D2}", min, sec);
    }

    void set_combo_name()
    {
        if (clip!=null)
        {
            combo_name.text = clip.name;
        }
        else
        {
            combo_name.text = "";
        }
    }

    void play_bell()
    {
        aus.PlayOneShot(bell);
    }

    void _set_state(State new_state)
    {
        _state = new_state;
        switch (new_state)
        {
            case State.wait_to_start:
                button.gameObject.SetActive(true);
                timer = 0;
                break;
            case State.countdown:
                timer = countdown_time;
                break;
            case State.bell:
                round_timer = round_time;
                play_bell();
                break;
            case State.play:
                select_sound();

                break;
            case State.idle:
                if (round_timer <= 0)
                {
                    play_bell();
                    timer = break_time;
                }
                else
                    timer = delay;
                break;

        }
    }
    void Start()
    {
        aus = gameObject.AddComponent<AudioSource>();
        cliplist = Resources.LoadAll<AudioClip>("Training/Sounds/");
        bell = Resources.Load<AudioClip>("Training/Sounds/bell");
        timer = countdown_time;
        round_timer = 0;
        state = State.wait_to_start;
    }

    void select_sound()
    {
        if (cliplist != null && cliplist.Length > 0)
        {
            clip = cliplist[Random.Range(0, cliplist.Length)];
            set_combo_name();
            aus.PlayOneShot(clip);
        }
    }

    public void start_flash()
    {
        button_animator.SetBool("start_flash", true);
        fade_in_layer.raycastTarget = false;

//         string url = "http://translate.google.com/translate_tts?tl=en&q=" + "start";
//         WWW www = new WWW(url);
//         yield return www;
//         AudioSource.PlayClipAtPoint(www.GetAudioClip(false, false, AudioType.MPEG), Vector3.zero);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        round_timer -= dt;
        switch (state)
        {
            case State.wait_to_start:
                {
                    var oldt = timer;
                    timer += dt;

                    var o = Mathf.Repeat(oldt, 1);
                    var t = Mathf.Repeat(timer, 1);

                    if (o<0.5f && t >=0.5f)
                    {
                        button_animator.SetTrigger("start_full");
                    }
                    else if (t<o)
                    {
                        button_animator.SetTrigger("start_x");
                    }
                    break;
                }
            case State.countdown:
                {
                    float oldt = timer;
                    timer -= dt;

                    set_gui_time(timer + 1);

                    if (timer <= 0)
                    {
                        state = State.bell;
                    }
                    break;
                }
            case State.bell:
                {
                    set_gui_time(round_timer + 1);
                    if (!aus.isPlaying)
                    {
                        state = State.play;
                    }
                    break;
                }
            case State.play:
                {
                    set_gui_time(round_timer + 1);
                    if (!aus.isPlaying)
                    {
                        state = State.idle;
                        clip = null;
                        set_combo_name();
                    }
                    break;
                }
            case State.idle:
                {
                    timer -= dt;
                    if (round_timer <= 0)
                        set_gui_time(timer + 1);
                    else
                        set_gui_time(round_timer + 1);

                    if (timer <= 0)
                    {
                        if (round_timer <= 0)
                        {
                            state = State.bell;
                        }
                        else
                        {
                            state = State.play;
                        }
                    }
                    break;
                }
        }
        /*
                if (timer <= 0)
                {
                    if (!aus.isPlaying)
                    {
                        timer = delay;
                        select_sound();
                    }
                }
                else
                {
                    timer -= Time.deltaTime;
                }
        */
    }
}
