//document.addEventListener('contextmenu', event => event.preventDefault());

document.addEventListener('DOMContentLoaded', function () {
    let content = document.getElementsByClassName("Indabo-Frame-Content")[0];
    let menu = document.getElementsByClassName("Indabo-Frame-Menu")[0];

    let scrollButtonUp = document.createElement("div");
    scrollButtonUp.classList.add("Indabo-Frame-Menu-Entry-ScrollButton");
    scrollButtonUp.classList.add("Indabo-Frame-Menu-Entry-ScrollButton-Up");
    scrollButtonUp.classList.add("Indabo-Frame-Menu-Entry-ScrollButton-Disabled");
    menu.append(scrollButtonUp);

    let scrollButtonDown = document.createElement("div");
    scrollButtonDown.classList.add("Indabo-Frame-Menu-Entry-ScrollButton");
    scrollButtonDown.classList.add("Indabo-Frame-Menu-Entry-ScrollButton-Down");

	let request = new XMLHttpRequest();
	request.open("GET", "./Panels");
	request.addEventListener("load", function() {
		if (request.status >= 200 && request.status < 300) {
            let panels = JSON.parse(request.responseText);

            console.log("Panels received: ", panels);

            let isFirst = true;
            let addMenuEntry = function(panelCounter) {     
                let panel = panels[panelCounter];

                let menuEntry = document.createElement("div");
                menuEntry.classList.add("Indabo-Frame-Menu-Entry");    

                let request = new XMLHttpRequest();
                request.open("GET", "./Panel/" + panel + ".png");
                request.addEventListener("load", function() {
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
                            request.addEventListener("load", function() {
                                if (request.status >= 200 && request.status < 300) {
                                    content.innerHTML = request.responseText;
                                }
                            });
                            request.send();
                        });

                        if (isFirst == true) {
                            isFirst = false;
                            document.getElementsByClassName("Indabo-Frame-Menu-Entry")[0].click();
                        }
                    }

                    if (panelCounter < panels.length - 1) {
                        addMenuEntry(panelCounter + 1);
                    }
                    else {
                        onPanelsLoaded();
                    }
                });
                request.send();
            }

            if (panels.length != 0) {
                addMenuEntry(0);
            }
		} 
		else {
			console.warn("Could not load panels!", request.statusText, request.responseText);
		}
	});
    request.send();

    let onPanelsLoaded = function() {
        menu.append(scrollButtonDown);
        checkMenuScrollable();
    }

    let checkMenuScrollable = function() {
        if (menu.scrollHeight > menu.clientHeight) {
            scrollButtonUp.style.display = "block";
            scrollButtonDown.style.display = "block";
        }
        else {
            scrollButtonUp.style.display = "none";
            scrollButtonDown.style.display = "none";
        }

        if (menu.scrollTop <= 0) {
            scrollButtonUp.classList.add("Indabo-Frame-Menu-Entry-ScrollButton-Disabled");
        }
        else {
            scrollButtonUp.classList.remove("Indabo-Frame-Menu-Entry-ScrollButton-Disabled");
        }

        if (menu.scrollTop >= (menu.scrollHeight - menu.clientHeight)) {
            scrollButtonDown.classList.add("Indabo-Frame-Menu-Entry-ScrollButton-Disabled");
        }
        else {
            scrollButtonDown.classList.remove("Indabo-Frame-Menu-Entry-ScrollButton-Disabled");
        }
    };   
    window.addEventListener("resize", checkMenuScrollable);
    menu.addEventListener("scroll", checkMenuScrollable);

    scrollButtonUp.addEventListener("click", function () {
        menu.scrollTop -= 40;        
    });

    scrollButtonDown.addEventListener("click", function () {
        menu.scrollTop += 40;        
    });
});