background =  {
    velocity: 1
}

class Player {
    constructor() {
        this.width = 100;
        this.height = 50;
        this.line = 2;
        this.lineHeight = 119
        this.position = {
            x: canvas.width - this.width - 50,
            y: this.line * this.lineHeight + this.lineHeight / 2 + this.height / 2
        }
        this.key = {
            w: false,
            s: false
        }
        this.isGameOver = false;
        this.randomImage = Math.floor(Math.random() * 4) + 1;
        this.image = new Image();
        this.image.src = `img/player${this.randomImage}.png`
    }

    update()
    {
        this.draw();
        this.move();
        this.checkCollisions();
    }

    draw()
    {
        ctx.save();
        ctx.translate(this.position.x + this.width / 2, this.position.y + this.height / 2);
        ctx.rotate(Math.PI * 1.5);
        const imageX = -this.image.width / 2;
        const imageY = -this.image.height / 2;
        ctx.scale(1.7, 1.5)
        if(this.image) ctx.drawImage(this.image, imageX, imageY);
        ctx.restore();
        //ctx.fillStyle = "rgba(255, 0, 0, 0.5)";
        //ctx.fillRect(this.position.x, this.position.y, this.width, this.height);
    }

    move()
    {
        if (this.isGameOver) return;
        if (this.key.w) 
        {
            if(this.line != 0) this.line--;
            else this.line = 0;
            this.key.w = false;
            gsap.to(this.position, {
                y: this.line * this.lineHeight + this.lineHeight / 2 + this.height / 2,
                duration: 0.3
            });
        }
        if (this.key.s) 
        {
            if(this.line != 3) this.line++;
            else this.line = 3;
            this.key.s = false;
            gsap.to(this.position, {
                y: this.line * this.lineHeight + this.lineHeight / 2 + this.height / 2,
                duration: 0.3
            });
        }
        
    }

    checkCollisions()
    {
        for (let i = 0; i < obstacles.length; i++)
        {
            const obstacle = obstacles[i];
            if (this.position.x + this.width >= obstacle.position.x &&
                this.position.x <= obstacle.position.x + obstacle.width &&
                this.position.y + this.height >= obstacle.position.y &&
                this.position.y <= obstacle.position.y + obstacle.height)
            {
                this.gameOver();
            }
        }
    }

    gameOver()
    {
        this.canMove = false
        gsap.to(this.position, {
            x: this.position.x + 20,
            duration: 2,
            ease: "expo.out"
        })
        
        gsap.to(background, {
            velocity: 0,
            duration: 3
        })

        gsap.to(Obstacle, {
            velocity: -2,
            ease: "power3.out",
            duration: 3,
            onComplete: () => {
                obstacles = [];
                this.isGameOver = true;
            }
        })
    }
}