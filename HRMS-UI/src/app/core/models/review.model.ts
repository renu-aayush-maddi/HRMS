export interface Review {
  id: string;
  employeeId: string;
  reviewerId: string;
  performanceCycleId: string;
  employeeName: string;
  reviewerName: string;
  performanceCycleName: string;
  rating?: number;
  comments?: string;
  reviewDate?: string;
  createdAt?: string;
}

export interface ReviewFilter {
  performanceCycleId?: string;
  employeeId?: string;
  reviewerId?: string;
  rating?: number;
  search?: string;
  minRating?: number;
  maxRating?: number;
  fromReviewDate?: string;
  toReviewDate?: string;
  sortBy?: string;
  descending?: boolean;
}
