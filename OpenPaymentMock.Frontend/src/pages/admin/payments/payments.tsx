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
        filterVariant: 'select',
        mantineFilterSelectProps: {
          data: [
            { value: 'Created', label: 'Created' },
            { value: 'Processing', label: 'Processing' },
            { value: 'Successed', label: 'Completed' },
            { value: 'Failed', label: 'Failed' },
            { value: 'Cancelled', label: 'Cancelled' },
            { value: 'Refunded', label: 'Refunded' },
          ],
        },
      },
      {
        id: 'amount',
        accessorFn: (_) => _.amount,
        header: 'Amount',
        filterVariant: 'range'
      },
      {
        id: 'currency',
        accessorFn: (_) => _.currency,
        header: 'Currency',
      },
      {
        id: 'createdAt',
        accessorFn: (_) => new Date(_.createdAt),
        header: 'Created At',
        sortingFn: 'datetime',
        Cell: ({ cell }) => (
          <span>{cell.getValue<Date>().toLocaleString()}</span>
        ),
        filterVariant: 'date-range',
      },
      {
        id: 'finishedAt',
        accessorFn: (_) => _.finishedAt ? new Date(_.finishedAt) : null,
        header: 'Finished At',
        sortingFn: 'datetime',
        Cell: ({ cell }) => cell.getValue<Date>() ? (
          <span>{cell.getValue<Date>().toLocaleString()}</span>
        ) : (<span>-</span>),
        filterVariant: 'date-range',
      },
      {
        id: 'partner',
        accessorFn: (_) => _.partnerId,
        Cell: ({ cell }) => {
          const partner = partners.find((_) => _.id === cell.getValue<string>());

          return (<span>
            {partner?.name ?? cell.getValue<string>()}
          </span>);
        },
        header: 'Partner',
        filterVariant: 'select',
        mantineFilterSelectProps: {
          data: partners.map((_) => ({
            value: _.id,
            label: _.name,
          })),
        }
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
      density: 'xs',
      showColumnFilters: true,
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

  return (<>
    <MantineReactTable table={table} />
  </>);
}
