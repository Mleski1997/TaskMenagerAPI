// import axios from "axios"
// import { getToken, getUser } from "./auth"

// const token = getToken();
// const userId = getUser();

// export async function fetchTasksUser(setTasks, setOrginalTask, setCompleted) {
// 	const response = await axios.get(`https://localhost:7219/api/User/${userId}/Todoes`, {
// 			headers: {
// 				Authorization: `Bearer ${token}`,
// 			},
// 	})

// 	if (response.status === 200) {
// 		setTasks(response.data)
// 		setOrginalTask(response.data)

// 		const completedTasks = JSON.parse(localStorage.getItem('completedTasks')) || []
// 		setCompleted(completedTasks)
// 	} else {
// 		console.log('Error with fetch task')
// 	}
// }