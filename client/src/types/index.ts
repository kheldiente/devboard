export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
}

export interface Board {
    id: string;
    name: string;
    projectId: string;
}

export type TaskStatus = 0 | 1 | 2; // Todo, InProgress, Done
export type TaskPriority = 0 | 1 | 2; // Low, Medium, High

export interface Task {
    id: string;
    title: string;
    description: string | null;
    status: TaskStatus;
    priority: TaskPriority;
    storyPoints: number;
    dueDate: string | null;
    assigneeId: string | null;
    assigneeName: string | null;
}

export interface CreateTaskRequest {
    title: string;
    description: string | null;
    boardId: string;
    assigneeId: string | null;
    storyPoints: number;
}