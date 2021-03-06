﻿using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public persistenceOwn persist;

	public float speed;
	public float jumpStrength;
	public float pogoStrength;
	public float gravity;
	public bool onPogo;
	public bool hasPogo;
	public bool hasGun;
	public bool isJumping;
	public GameObject bullet;
	public GameObject respawnPoint;
	public AudioClip dieSound;

	public bool deadAnimation;
	public int deadAnimCount;
	 
	private Animator anim;
	private CharacterController p;
	public Vector3 dir = new Vector3();
	public int ammunition = 5;
	public int score = 0;
	public int highscore;
	public int lives = 3;
	public bool hitHead;


	// Use this for initialization
	void Start () {
		anim = (Animator)this.GetComponent ("Animator");
		p = (CharacterController)(this.GetComponent("CharacterController"));
		this.hasPogo = true;
		this.hasGun = true;
		//this.respawn ();
		if (Application.loadedLevel > 1) {
			persist.SendMessage ("loadPlayer");
		} else {
			persist.SendMessage("loadHighScore");
		}
	}
	
	// Update is called once per frame
	void Update () {

		// check if still alive 
		if (lives == 0) {
			Application.LoadLevel("GameOverScreen");
		}

		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, -2);
		if (deadAnimation) {

			if (deadAnimCount > 0) {
				Vector3 actPos = this.transform.position;
				this.transform.position = new Vector3 (actPos.x, actPos.y + 0.02f, actPos.z);
				deadAnimCount--;
			} else {
				anim.SetBool ("dead", false);
				deadAnimation = false;
				this.transform.position = this.respawnPoint.transform.position;
				this.transform.rotation = new Quaternion(0,180,0,0);

			}
		} else {
				deadAnimCount = 120;
				RaycastHit hit;
				Physics.Raycast (this.transform.position, Vector3.up, out hit);
				if (hit.distance < 1 && !this.hitHead && hit.distance != 0 && hit.collider.tag != "enemy" && hit.collider.tag != "crate") {
					dir.y = 0;
					this.hitHead = true;
				} 
				// Handle Movement
				if (p.isGrounded) {
						dir.y = 0;
						this.isJumping = false;
				}
				dir.x = Input.GetAxis ("Horizontal") * speed;
				if (Input.GetAxis ("Horizontal") > 0) {
						transform.rotation = new Quaternion (0, 180, 0, 0);
				} else if (Input.GetAxis ("Horizontal") < 0) {
						transform.rotation = new Quaternion (0, 0, 0, 0);
				}
				if (Input.GetButtonDown ("Jump") && (this.onPogo || this.p.isGrounded) && !this.isJumping ) {
						if(this.onPogo)
							this.isJumping = true;
						
						this.onPogo = false;
						anim.SetBool ("onPogo", this.onPogo);
						dir.y = this.jumpStrength;
				}

				if (Input.GetButtonDown ("Pogo")) {

						this.onPogo = !this.onPogo;

						if (!this.onPogo) {
								dir.y = 0;
						}
						anim.SetBool ("onPogo", this.onPogo);
				}

				// Handle Shooting
				if (Input.GetButtonDown ("Fire1")) {
						if (this.ammunition > 0) {
								anim.SetTrigger ("shot");
								this.shoot ();
						}
				}

				// Handle Animations
				if (dir.x != 0) {
						anim.SetBool ("moving", true);
				} else {
						anim.SetBool ("moving", false);
				}
				if (p.isGrounded) {
						this.hitHead = false;
						if (onPogo) {
								dir.y = pogoStrength;

						} else {
								anim.SetBool ("isGrounded", true);
						}
				} else {
						anim.SetBool ("isGrounded", false);
				}
				dir.y -= gravity * Time.deltaTime;
				p.Move (dir * Time.deltaTime);
		}
	}

	void shoot() {
		this.ammunition--;
		if (this.transform.rotation.y == 0) {
				GameObject b = (GameObject)Instantiate (this.bullet, 
		                                   new Vector3 (this.transform.position.x - 1, this.transform.position.y, -2), 
		                                   Quaternion.identity);
				b.SendMessage ("goLeft");
		} else {
			GameObject b = (GameObject)Instantiate (this.bullet, 
			                                        new Vector3 (this.transform.position.x + 1, this.transform.position.y, -2), 
			                                        Quaternion.identity);
		}
	}

	void addAmmunition(int amount) {
		this.ammunition += amount;
	}

	void respawn() {
		AudioSource.PlayClipAtPoint(this.dieSound, this.transform.position);
		this.lives--;
		this.onPogo = false;
		anim.SetBool ("dead", true);
		anim.SetBool ("isGrounded", false);
		anim.SetBool ("onPogo", false);

		this.deadAnimation = true;
	}

	void addScore(int amount) {
		this.score += amount;
	}

}
