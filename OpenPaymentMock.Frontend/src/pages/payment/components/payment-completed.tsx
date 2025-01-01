import { SchemaPaymentSituationPublicDto } from '@/lib/openpaymentmock-backend';
import { Check, X } from 'lucide-react';
import { Link } from 'react-router-dom';

export default function PaymentCompleted({
  payment,
}: {
  payment: SchemaPaymentSituationPublicDto;
}) {
  return (
    <div className="flex flex-col items-center justify-center h-screen bg-gray-100 dark:bg-gray-950">
      <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md">
        <div className="flex flex-col items-center justify-center space-y-6">
          {payment.status === 'Succeeded' && (
            <>
              <div className="bg-green-500 rounded-full p-4">
                <Check className="h-12 w-12 text-white" />
              </div>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
                Payment Successful
              </h1>
            </>
          )}

          {payment.status === 'Cancelled' && (
            <>
              <div className="bg-gray-500 rounded-full p-4">
                <X className="h-12 w-12 text-white" />
              </div>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
                Payment Cancelled
              </h1>
            </>
          )}

          {payment.status === 'Failed' && (
            <>
              <div className="bg-red-500 rounded-full p-4">
                <X className="h-12 w-12 text-white" />
              </div>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-50">
                Payment Failed
              </h1>
            </>
          )}

          <div className="space-y-2 text-center">
            <p className="text-gray-500 dark:text-gray-400">
              Your payment of{' '}
              <span className="font-medium">
                {payment.amount} ({payment.currency})
              </span>{' '}
              to <span className="font-medium">{payment.partnerName}</span> was{' '}
              {payment.status === 'Succeeded' && <span>successful.</span>}
              {payment.status === 'Cancelled' && <span>cancelled.</span>}
              {payment.status === 'Failed' && <span>aborted.</span>}
            </p>
            <p className="text-gray-500 dark:text-gray-400">
              Transaction Date:{' '}
              <span className="font-medium">{new Date(payment.finishedAt ?? new Date()).toLocaleString()}</span>
            </p>
          </div>
          <div className="flex flex-col gap-2 w-full">
            <Link
              to={payment.redirectUrl}
              className="text-center text-blue-600 hover:underline"
            >
              Return to vendor
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
