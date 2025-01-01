import { FileQuestion } from "lucide-react"

export default function PaymentNotFound() {
    return (
        <div className="flex flex-col items-center justify-center h-screen bg-gray-100 dark:bg-gray-950">
          <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md">
            <div className="flex flex-col items-center justify-center space-y-6">
              
                  <div className="bg-red-500 rounded-full p-4">
                    <FileQuestion className="h-12 w-12 text-white" />
                  </div>
                  <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
                    Payment not found
                  </h1>
    
              <div className="space-y-2 text-center">
                <p className="text-gray-500 dark:text-gray-400">
                    The payment you are looking for does not exist.
                </p>
              </div>
            </div>
          </div>
        </div>
    )
}
