async function getBotResponse(input) {
    let  x = "";
    await fetch('http://18.159.111.193/api/Review/chatbots/customerService/'+input)
        .then(response => response.text())
        .then(text => {  x= text ; });

    return x;
    //rock paper scissors
    if (input == "rock") {
        return "paper";
    } else if (input == "paper") {
        return "scissors";
    } else if (input == "scissors") {
        return "rock";
    }

    // Simple responses
    if (input == "hello") {
        return "Hello there!";
    } else if (input == "goodbye") {
        return "Talk to you later!";
    } else {
        return "Try asking something else!";
    }

}

