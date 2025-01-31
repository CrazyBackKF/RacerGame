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

function checkClick(e)
{
    const mouseX = e.offsetX;
    const mouseY = e.offsetY;
    let canvasRect = canvas.getBoundingClientRect(); // Pobieranie pozycji canvasu na ekranie
    let scaleX = canvas.width / canvasRect.width; // Skala w poziomie
    let scaleY = canvas.height / canvasRect.height; // Skala w pionie

    let mouseXRelative = (mouseX - canvasRect.left) * scaleX;
    let mouseYRelative = (mouseY - canvasRect.top) * scaleY;

    return ((mouseX <= ((canvas.width - 260) / 2 + 250) && mouseX >= (canvas.width - 260) / 2 &&
    mouseY <= canvas.height - 150 + 100 && mouseY >= canvas.height - 150) || (mouseXRelative <= ((canvas.width - 260) / 2 + 250) && mouseXRelative >= (canvas.width - 260) / 2 &&
    mouseYRelative <= canvas.height - 150 + 100 && mouseYRelative >= canvas.height - 150))
}