# Getting started with Chat Events
1. Clone this repository
2. Clone the frontend repository which can be found here: [frontend repo](https://github.com/Marc19/chatEvents-frontend/)
3. Start both projects, make sure the backend runs on `port 5001`. Otherwise you can update the port number in the frontend, which can be found in `src/configuration.js`

# Testing Chat Events
- You will find pre-existing `4 users` and `1 chat room`, just for simplicity
- You will also find some pre-existing `events` with dates `20/12/2020` and `20/11/2020`
- You can start performing actions for users from the frontend (ex: enter-the-room, comment, ...), which in turn will trigger corresponding events, and these events can be seen in the chat room
- When viewing events, you can change the granularity which can be `minute by minute` (which will list every chat event as it occurred), or during an `interval of time` (which will display event stats during each interval)
- You can also choose a date range to limit your query to those dates by providing a `from date` and/or a `to date`