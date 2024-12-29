import { useMemo } from 'react';
import {
  MantineReactTable,
  useMantineReactTable,
  type MRT_ColumnDef,
} from 'mantine-react-table';
import { SchemaPaymentSituationDetailsDto } from '@/lib/openpaymentmock-backend';
import { useBackendQuery } from '@/lib/openpaymentmock-client';
import { useAuthentication } from '@/contexts/authentication-context';
import { Button } from '@/components/ui/button';
import {
  TooltipProvider,
  Tooltip,
  TooltipTrigger,
  TooltipContent,
} from '@/components/ui/tooltip';
import { CreditCard } from 'lucide-react';
import { Link } from 'react-router-dom';

export default function PaymentsPage() {
  const { apiKey } = useAuthentication();

  const { data, isLoading, isValidating, error, mutate } = useBackendQuery(
    '/api/payments',
    {
      headers: {
        'X-Api-Key': apiKey,
      },
    },
  );

  const { data: partnersData } = useBackendQuery('/api/partners', {
    headers: {
      'X-Api-Key': apiKey,
    },
  });

  const partners = useMemo(() => partnersData ?? [], [partnersData]);

  const columns = useMemo<
    MRT_ColumnDef<SchemaPaymentSituationDetailsDto>[]
  >(() => {
    const columns: MRT_ColumnDef<SchemaPaymentSituationDetailsDto>[] = [
      {
        id: 'id',
        accessorFn: (_) => _.id,
        header: 'Id',
        enableHiding: true,
      },
      {
        id: 'status',
        accessorFn: (_) => _.status,
        header: 'Status',
      },
      {
        id: 'amount',
        accessorFn: (_) => _.amount,
        header: 'Amount',
      },
      {
        id: 'currency',
        accessorFn: (_) => _.currency,
        header: 'Currency',
      },
      {
        id: 'createdAt',
        accessorFn: (_) => _.createdAt,
        header: 'Created At',
        sortingFn: 'datetime',
        Cell: ({ cell }) => (
          <span>{new Date(cell.getValue<string>()).toLocaleString()}</span>
        ),
      },
      {
        id: 'finishedAt',
        accessorFn: (_) => _.finishedAt,
        header: 'Finished At',
        sortingFn: 'datetime',
        Cell: ({ cell }) => cell.getValue<string>() ? (
          <span>{new Date(cell.getValue<string>()).toLocaleString()}</span>
        ) : (<span>-</span>),
      },
      {
        id: 'partner',
        accessorFn: (_) => {
          const partner = partners.find((__) => __.id === _.partnerId);

          return partner?.name ?? _.partnerId;
        },
        header: 'Partner',
      },
    ];

    return columns;
  }, [partners]);

  const table = useMantineReactTable({
    columns,
    data: data ?? [],
    getRowId: (row) => row.id,
    state: {
      isLoading,
      showProgressBars: isValidating,
      showAlertBanner: error !== undefined,
    },
    renderTopToolbarCustomActions: () => (
      <div className="flex flex-row gap-2">
        <Button
          variant="secondary"
          onClick={() => {
            mutate();
          }}
        >
          Refresh data
        </Button>
      </div>
    ),
    createDisplayMode: undefined,
    editDisplayMode: undefined,
    enableGrouping: true,
    initialState: {
      grouping: ['partner', 'status'],
      expanded: true,
      columnVisibility: {
        id: false,
      },
    },
    enableRowActions: true,
    positionActionsColumn: 'last',
    renderRowActions: ({ row }) => (
      <div className="flex flex-row gap-2">
        <TooltipProvider>
          <Tooltip>
            <TooltipTrigger>
              <Link to={`/payments/${row.original.id}`} target='_blank'>
                <CreditCard />
              </Link>
            </TooltipTrigger>
            <TooltipContent>Open payment page</TooltipContent>
          </Tooltip>
        </TooltipProvider>
      </div>
    ),
  });

  return <MantineReactTable table={table} />;
}
