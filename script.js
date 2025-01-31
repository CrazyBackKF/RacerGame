const canvas = document.querySelector("canvas");
const ctx = canvas.getContext("2d");

const player = new Player();

let obstacles = [];
let lastDate = 0;
let multiplier = 1;
let score = 1;
let highScore = localStorage.getItem("highScore") | 0;
let frame = 0;

function isFullscreen() {
    return document.fullscreenElement !== null || 
           document.webkitFullscreenElement !== null ||
           document.mozFullScreenElement !== null ||
           document.msFullscreenElement !== null;
}

function animate()
{
    
    requestAnimationFrame(animate);
    if (player.isGameOver)
    {
        if (score > highScore)
        {
            highScore = score;
            localStorage.setItem("highScore", JSON.stringify(highScore));
        }
        ctx.fillStyle = "black"
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = "red"
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.font = "50px 'Press Start 2P'"
        ctx.fillText("Game over!", canvas.width / 2, (canvas.height - 200) / 2);
        ctx.font = "20px 'Press Start 2P'"
        ctx.fillText(`Your score is ${score}`, canvas.width / 2, canvas.height / 2);
        ctx.fillText(`Your High Score is ${highScore}`, canvas.width / 2, (canvas.height + 200) / 2);
        ctx.fillStyle = "red"
        ctx.fillRect((canvas.width - 260) / 2, canvas.height - 150, 250, 100)
        ctx.fillStyle = "black";
        ctx.fillText("Play again!", canvas.width / 2, canvas.height - 100)

    }
    
    if (!player.isGameOver)
    {
        drawBackground();

        if (Math.random() < 0.01 && Date.now() - lastDate >= (1500 * (1 - (multiplier / 5))))
        {
            const line = Math.floor(Math.random() * 4);
            const randomImage = Math.floor(Math.random() * 12) + 1
            const scale = {
                x: 0,
                y: 0
            }
            if (randomImage >= 1 && randomImage <= 4)
            {
                scale.x = carType.compact.scaleX;
                scale.y = carType.compact.scaleY;
            }
            if (randomImage >= 5 && randomImage <= 8)
            {
                scale.x = carType.coupe.scaleX;
                scale.y = carType.coupe.scaleY;
            }
            if (randomImage >= 9 && randomImage <= 12)
            {
                scale.x = carType.sedan.scaleX;
                scale.y = carType.sedan.scaleY;
            }
            const imageSrc = `img/${randomImage}.png`
            obstacles.push(new Obstacle(line, scale, imageSrc))
            lastDate = Date.now();
        }

        for (let i = 0; i < obstacles.length; i++)
        {
            obstacles[i].update();
            if (obstacles[i].position.x == canvas.width) obstacles.splice(i, 1)
        }

        player.update();
        ctx.fillStyle = "black";
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.font = "30px 'Press Start 2P'"
        ctx.fillText(score, 100, 40)
        if(frame % 10 == 0) 
        {
            score++;
            if (score % 100 == 0) multiplier += 0.2;
        }
        
        frame++;
        ctx.font = "30px 'Press Start 2P'"
        ctx.fillText(`High Score ${highScore}`, canvas.width - 300, 40)
    }    
}

animate();

addEventListener("keydown", (e) => {
    switch(e.key.toLowerCase())
    {
        case 'w':
            player.key.w = true;
            break;
        case 's':
            player.key.s = true;
            break;
    }
})

canvas.addEventListener("click", (e) => {
    if (!player.isGameOver) return;
    if (checkClick(e))
    {
        player.randomImage = Math.floor(Math.random() * 4) + 1;
        player.image.src = `img/player${player.randomImage}.png`
        background.velocity = 1;
        Obstacle.velocity = 1;
        player.position.x = canvas.width - player.width - 50;
        score = 1;
        lastDate = 0;
        multiplier = 1;
        frame = 0;
        player.line = 2;
        player.position.y = player.line * player.lineHeight + player.lineHeight / 2 + player.height / 2
        player.canMove = true;
        player.isGameOver = false;
        canvas.style.cursor = "default"
    }
})

canvas.addEventListener("mousemove", (e) => {
    if (!player.isGameOver) return;
    if (checkClick(e))
    {
        canvas.style.cursor = "pointer"
    }
    else canvas.style.cursor = "default"
})

document.getElementById("fullscreen").addEventListener("click", () => {
    canvas.requestFullscreen();
})

