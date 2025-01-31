function drawBackground()
{
    ctx.fillStyle = "green";
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = "rgb(50, 50, 50)";
    ctx.fillRect(0, 50, canvas.width, canvas.height - 100);
    ctx.strokeStyle = "white"
    ctx.lineWidth = 5;
    ctx.setLineDash([10, 3]);
    ctx.lineDashOffset -= (background.velocity * multiplier);
    ctx.beginPath();
    ctx.moveTo(0, 50);
    ctx.lineTo(canvas.width, 50);
    ctx.stroke();
    
    ctx.beginPath();
    ctx.moveTo(0, canvas.height - 50);
    ctx.lineTo(canvas.width, canvas.height - 50);
    ctx.stroke();
    
    ctx.setLineDash([]);
    ctx.beginPath();
    ctx.moveTo(0, canvas.height / 2);
    ctx.lineTo(canvas.width, canvas.height / 2);
    ctx.stroke();
    
    ctx.setLineDash([20, 10]);
    ctx.lineDashOffset -= (background.velocity * multiplier);
    ctx.beginPath();
    ctx.moveTo(0, (canvas.height / 2 + 50) / 2);
    ctx.lineTo(canvas.width, (canvas.height / 2 + 50) / 2);
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(0, canvas.height / 2 + (canvas.height / 2 - 50) / 2);
    ctx.lineTo(canvas.width, canvas.height / 2 + (canvas.height / 2 - 50) / 2);
    ctx.stroke();

}