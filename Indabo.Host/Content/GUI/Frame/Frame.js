//document.addEventListener('contextmenu', event => event.preventDefault());

document.addEventListener('DOMContentLoaded', function () {
    let content = document.getElementsByClassName("Indabo-Frame-Content")[0];
    let menu = document.getElementsByClassName("Indabo-Frame-Menu")[0];

	let request = new XMLHttpRequest();
	request.open("GET", "./Panels");
	request.addEventListener("load", function() {
		if (request.status >= 200 && request.status < 300) {
            let panels = JSON.parse(request.responseText);

            console.log("Panels received: ", panels);

            for (let panel of panels) {                
                let menuEntry = document.createElement("div");
                menuEntry.classList.add("Indabo-Frame-Menu-Entry");                

                let request = new XMLHttpRequest();
                request.open("GET", "./Panel/" + panel + ".png");
                request.addEventListener("load", () => {
                    if (request.status >= 200 && request.status < 300) {
                        menuEntry.style.backgroundImage = "url('./Panel/" + panel + ".png')";
                        menu.append(menuEntry);

                        menuEntry.addEventListener("click", function () {
                            let otherElements = document.getElementsByClassName("Indabo-Frame-Menu-Entry");

                            for (otherElement of otherElements) {
                                otherElement.classList.remove("Indabo-Frame-Menu-Entry-Selected");     
                            }

                            menuEntry.classList.add("Indabo-Frame-Menu-Entry-Selected");     

                            let request = new XMLHttpRequest();
                            request.open("GET", "./Panel/" + panel + ".html");
                            request.addEventListener("load", () => {
                                if (request.status >= 200 && request.status < 300) {
                                    content.innerHTML = request.responseText;
                                }
                            });
                            request.send();
                        });
                    }
                });
                request.send();
            }
		} 
		else {
			console.warn("Could not load panels!", request.statusText, request.responseText);
		}
	});
	request.send();

});