const carType = {
    compact: {
        scaleX: 2,
        scaleY: 2
    },

    coupe: {
        scaleX: 1.7,
        scaleY: 1.5
    },

    sedan: {
        scaleX: 1.7,
        scaleY: 1.5
    }
}




class Obstacle {
    constructor(line, scale, imageSrc)
    {
        this.width = 100,
        this.height = 50;
        this.velocity = 1
        this.lineHeight = canvas.height / 5;
        this.scale = scale;
        this.line = line;
        this.position = {
            x: 0 - this.width,
            y: this.line * this.lineHeight + (this.lineHeight / 2 - this.height / 2)
        }
        this.image = new Image()
        this.image.src = imageSrc;
    }

    update()
    {
        this.draw();
        this.move();
    }
    
    draw()
    {
        ctx.save();
        ctx.translate(this.position.x + this.width / 2, this.position.y + this.height / 2);
        ctx.rotate(Math.PI * 1.5);
        const imageX = -this.image.width / 2;
        const imageY = -this.image.height / 2;
        ctx.scale(this.scale.x, this.scale.y);
        if(this.image) ctx.drawImage(this.image, imageX, imageY);
        ctx.restore();
        //ctx.fillStyle = "rgba(0,0,255,0.5)";
        //ctx.fillRect(this.position.x, this.position.y, this.width, this.height);
    }

    move()
    {

        this.position.x += (this.velocity * multiplier)
    }
}