class Player {
    constructor() {
        this.width = 100,
        this.height = 50
        this.position = {
            x: canvas.width - this.width - 50,
            y: (canvas.height - this.height) / 2
        }
        this.key = {
            w: false,
            s: false
        }
        this.line = 2;
        this.lineHeight = canvas.height / 5;
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
                y: this.line * this.lineHeight + (this.lineHeight / 2 - this.height / 2),
                duration: 0.3
            });
        }
        if (this.key.s) 
        {
            if(this.line != 4) this.line++;
            else this.line = 4;
            this.key.s = false;
            gsap.to(this.position, {
                y: this.line * this.lineHeight + (this.lineHeight / 2 - this.height / 2),
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
        obstacles = [];
        this.isGameOver = true;
    }
}