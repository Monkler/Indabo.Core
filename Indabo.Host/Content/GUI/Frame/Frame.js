//document.addEventListener('contextmenu', event => event.preventDefault());

document.addEventListener('DOMContentLoaded', function () {
    let widgets = [];
    var scriptTags = document.getElementsByTagName('script');
    for (let widget of scriptTags) {
        if (widget.src.startsWith(window.location.origin + "/Widget/")) {
            widgets.push(widget.src.substring((window.location.origin + "/Widget/").length, widget.src.length - 3));
        }
    }

    console.log("Widgets loaded: ", widgets);

    let updateWidgets = function () {
        for (let widget of widgets) {
            let widgetClass = eval(widget);

            let widgetElements = document.querySelectorAll('[indabo-widget="' + widget + '"]');
            for (let widgetElement of widgetElements) {
                new widgetClass(widgetElement);
            }
        }
    }

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

            let addMenuEntry = function(panelCounter) {     
                let panel = panels[panelCounter];

                let menuEntry = document.createElement("div");
                menuEntry.id = "indabo-panel-" + panel;
                menuEntry.classList.add("Indabo-Frame-Menu-Entry");    

                let request = new XMLHttpRequest();
                request.open("GET", "./Panel/" + panel + ".png");
                request.addEventListener("load", function() {
                    if (request.status >= 200 && request.status < 300) {
                        menuEntry.style.backgroundImage = "url('./Panel/" + panel + ".png')";
                        menu.append(menuEntry);

                        menuEntry.addEventListener("click", function () {
                            window.location = "#" + panel;

                            let otherElements = document.getElementsByClassName("Indabo-Frame-Menu-Entry");

                            for (otherElement of otherElements) {
                                otherElement.classList.remove("Indabo-Frame-Menu-Entry-Selected");
                            }

                            menuEntry.classList.add("Indabo-Frame-Menu-Entry-Selected");

                            content.innerHTML = "";
                            let request = new XMLHttpRequest();
                            request.open("GET", "./Panel/" + panel + ".html");
                            request.addEventListener("load", function () {
                                if (request.status >= 200 && request.status < 300) {
                                    content.innerHTML = request.responseText;
                                    updateWidgets();
                                }
                            });
                            request.addEventListener("error", function () {
                                content.innerHTML = "";
                            });
                            request.send();
                        });
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

        let selectedPanel = unescape(window.location.href.substring((window.location.origin).length + 2, window.location.href.length));

        let found = false;
        for (let menuEntry of document.getElementsByClassName("Indabo-Frame-Menu-Entry")) {
            console.log(menuEntry.id, "indabo-panel-" + selectedPanel);
            if (menuEntry.id == "indabo-panel-" + selectedPanel) {
                found = true;
                menuEntry.scrollIntoView();

                if (Math.ceil(menu.scrollTop) < (menu.scrollHeight - menu.clientHeight)) {
                    menu.scrollTop -= 40;
                }

                menuEntry.click();
            }
        }

        if (found == false) {
            document.getElementsByClassName("Indabo-Frame-Menu-Entry")[0].click();
        }
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

        if (Math.ceil(menu.scrollTop) >= (menu.scrollHeight - menu.clientHeight)) {
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